import torch
import gymnasium as gym
import numpy as np
import torch.nn as nn
import random
from collections import deque
import torch.optim as optim
import time

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



env=gym.make("LunarLander-v3",render_mode="human")

obs_dim=env.observation_space.shape[0]
action_dim=env.action_space.n

loaded_network=QNetwork(obs_dim=obs_dim,action_dim=action_dim)
state_dict=torch.load("best_model.pth",weights_only=True)
loaded_network.load_state_dict(state_dict)
loaded_network.eval()

for episode in range(5):
    obs,info=env.reset()
    total_reward=0
    while True:
        state_tensor = torch.tensor(obs, dtype=torch.float32)
        with torch.no_grad():
            q_values = loaded_network(state_tensor)
        action = torch.argmax(q_values).item()
        obs, reward, terminated, truncated, info = env.step(action)
        total_reward+=reward
        if terminated or truncated:
            break
        time.sleep(0.01)
    print(f"エピソード {episode + 1}, 報酬: {total_reward}")

env.close()