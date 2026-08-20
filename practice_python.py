import gymnasium as gym
import numpy as np
import torch

obs_bounds=[
    (-2.4,2.4),
    (-3.0,3.0),
    (-0.5,0.5),
    (-3.5,3.5),
]
n_bins=[6,6,12,12]


bins=[np.linspace(low,high,n+1)[1:-1] 
      for (low,high),n in zip(obs_bounds,n_bins)]

env= gym.make("CartPole-v1", render_mode="human")
obs,info= env.reset()
discretized=[]
for i,value in enumerate(obs):
    discretized.append(np.digitize(value,bins[i])-1)
print("PyTorch version:", torch.__version__)