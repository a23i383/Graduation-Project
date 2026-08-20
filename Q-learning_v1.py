import gymnasium as gym
import numpy as np

env = gym.make("CartPole-v1")

obs_bounds = [
    (-2.4, 2.4), 
    (-3.0, 3.0), 
    (-0.5, 0.5), 
    (-3.5, 3.5)
    ]
n_bins = [6, 6, 12, 12]
bins = [np.linspace(low, high, n + 1)[1:-1] for (low, high), n in zip(obs_bounds, n_bins)]

alpha=0.1
gamma=0.99
epsilon=1.0
epsilon_min=0.01
epsilon_decay=0.999
n_episodes=2000
sum_reward=0

def discretize(obs, bins):
    discretized = []
    for i, value in enumerate(obs):
        discretized.append(np.digitize(value, bins[i]))
    return tuple(discretized)

q_table_shape=n_bins+[env.action_space.n]
q_table=np.zeros(q_table_shape)

def update_q_table(q_table,state,action,reward,next_state,alpha,gamma,terminated):
    current_q=q_table[state][action]
    if terminated:
        target_q=reward
    else:
        max_next_q=np.max(q_table[next_state])
        target_q=reward+gamma*max_next_q

    q_table[state][action]=current_q+alpha*(target_q-current_q)

def choose_action(q_table,state,epsilon,env):
    if np.random.rand()<epsilon:
        action = env.action_space.sample()
    else:
        action = np.argmax(q_table[state])
    return action

for episode in range(n_episodes):
    obs,info=env.reset()
    state=discretize(obs,bins)

    total_reward=0
    
    for step in range(500):
        action=choose_action(q_table,state,epsilon,env)
        obs,reward,terminated,truncated,info=env.step(action)
        next_state=discretize(obs,bins)
        update_q_table(q_table,state,action,reward,next_state,alpha,gamma,terminated)
        state=next_state
        total_reward+=reward
        if terminated or truncated:
            break
    epsilon=max(epsilon_min,epsilon*epsilon_decay)

    sum_reward+=total_reward
    if(episode+1)%100==0:
        print(f"Episode: {episode+1}, AverageReward: {sum_reward/100:.3f}, Epsilon: {epsilon:.3f}")
        sum_reward=0

env.close()