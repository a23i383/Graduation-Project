import torch
import gymnasium as gym
import numpy as np
import torch.nn as nn

class DQN(nn.Module):
    def __init__(self,obs_dim,action_dim):
        super().__init__()

        self.layers=nn.Sequential(
            nn.Linear(obs_dim,64),
            nn.ReLU(),
            nn.Linear(64,64),
            nn.ReLU(),
            nn.Linear(64,action_dim)
        )

    def forward(self,x):
        return self.layers(x)


q_network=DQN(obs_dim=4,action_dim=2)
dummy_obs = torch.tensor([0.01, -0.02, 0.03, 0.01], dtype=torch.float32)
q_values = q_network(dummy_obs)

print("入力(observation):", dummy_obs)
print("出力(各行動のQ値):", q_values)
print("一番Q値が高い行動:", torch.argmax(q_values).item())