import gymnasium as gym
import time

env=gym.make("CartPole-v1",render_mode="human")
observation,info=env.reset()

print("action_space:",env.action_space)
print("action_space.n:",env.action_space.n)
print("observation_space:",env.observation_space)

