import gymnasium as gym
import numpy as np
import torch
import torch.nn as nn
import torch.optim as optim

env=gym.make("CartPole-v1")
obs_dim=env.observation_space.shape[0]
action_dim=env.action_space.n
print(action_dim)

