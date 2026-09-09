import torch
import gymnasium as gym
import numpy as np
import torch.nn as nn
import random
from collections import deque
import torch.optim as optim
import time

def evaluate(q_network, env, n_episodes=10):
    total_rewards = []
    for _ in range(n_episodes):
        obs, info = env.reset()
        episode_reward = 0
        for step in range(500):
            state_tensor = torch.tensor(obs, dtype=torch.float32)
            with torch.no_grad():
                q_values = q_network(state_tensor)
            action = torch.argmax(q_values).item()

            obs, reward, terminated, truncated, info = env.step(action)
            episode_reward += reward
            if terminated or truncated:
                break
        total_rewards.append(episode_reward)
    return np.mean(total_rewards)

class ReplayBuffer:
    def __init__(self,capacity):
        self.buffer=deque(maxlen=capacity)

    def push(self,state,action,reward,next_state,terminated):
        self.buffer.append((state,action,reward,next_state,terminated))

    def sample(self,batch_size):
        batch=random.sample(self.buffer,batch_size)
        states,actions,rewards,next_states,terminateds=zip(*batch)
        return states,actions,rewards,next_states,terminateds

    def __len__(self):
        return len(self.buffer)

class QNetwork(nn.Module):
    def __init__(self,obs_dim,action_dim):
        super().__init__()

        self.layers=nn.Sequential(
            nn.Linear(obs_dim,64),
            nn.ReLU(),
            nn.Linear(64,128),
            nn.ReLU(),
            nn.Linear(128,64),
            nn.ReLU(),
            nn.Linear(64,action_dim)
        )

    def forward(self,x):
        return self.layers(x)

def train_step(q_network,target_network,optimizer,
               loss_fn,buffer,batch_size,gamma):
    if len(buffer)<batch_size:
        return

    states,actions,rewards,next_states,terminateds=buffer.sample(batch_size)
    states=torch.tensor(states,dtype=torch.float32)
    actions=torch.tensor(actions,dtype=torch.int64)
    rewards=torch.tensor(rewards,dtype=torch.float32)
    next_states=torch.tensor(next_states,dtype=torch.float32)
    terminateds=torch.tensor(terminateds,dtype=torch.float32)

    all_q_values=q_network(states)
    current_q=all_q_values.gather(1,actions.unsqueeze(1))
    current_q=current_q.squeeze(1)

    with torch.no_grad():
        next_q_values=target_network(next_states)
        max_next_q=next_q_values.max(1)[0]
        target_q=rewards+gamma*max_next_q*(1-terminateds)

    loss=loss_fn(current_q,target_q)
    optimizer.zero_grad()
    loss.backward()
    optimizer.step()

    return loss.item()

env=gym.make("LunarLander-v3")
obs_dim=env.observation_space.shape[0]
action_dim=env.action_space.n

q_network=QNetwork(obs_dim=obs_dim,action_dim=action_dim)
target_network=QNetwork(obs_dim=obs_dim,action_dim=action_dim)
target_network.load_state_dict(q_network.state_dict())

optimizer=optim.Adam(q_network.parameters(),lr=0.001)
loss_fn=nn.MSELoss()
buffer=ReplayBuffer(capacity=10000)

gamma=0.99
batch_size=64
epsilon=1.0
epsilon_min=0.01
epsilon_decay=0.995
target_update_freq=10
n_episodes=600

def choose_action_dqn(q_network,state,epsilon,env):
    if np.random.rand()<epsilon:
        action=env.action_space.sample()
    else:
        state_tensor=torch.tensor(state,dtype=torch.float32)
        with torch.no_grad():
            q_values=q_network(state_tensor)
        action=torch.argmax(q_values).item()
    return action

sum_total_reward=0
best_score=-10000
for episode in range(n_episodes):
    obs,info=env.reset()
    total_reward=0

    while True:
        action=choose_action_dqn(q_network,obs,epsilon,env)
        next_obs,reward,terminated,truncated,info=env.step(action)

        buffer.push(obs,action,reward,next_obs,terminated)

        train_step(q_network,target_network,
                   optimizer,loss_fn,buffer,batch_size,gamma)

        obs=next_obs
        total_reward+=reward

        if terminated or truncated:
            break
    epsilon=max(epsilon_min,epsilon*epsilon_decay)
    sum_total_reward+=total_reward

    if(episode+1)%target_update_freq==0:
        target_network.load_state_dict(q_network.state_dict())

    if(episode+1)%40==0:
        eval_score = evaluate(q_network, env)
        print(f"評価スコア(epsilon=0): {eval_score}")
        if eval_score >= best_score:
            best_score = eval_score
            torch.save(q_network.state_dict(), "best_model.pth") 
            print(f"→ ベストモデルを更新して保存しました(score={eval_score})")
        print(f"エピソード {episode + 1}, 平均報酬: {sum_total_reward/40}, epsilon: {epsilon:.3f}")
        sum_total_reward=0



env.close()
