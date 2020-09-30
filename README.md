# MARL-in-mixed-setting-




The aim of this project is to smulate a popular children's game,  "Seven Tiles" in order to observe and compare the interaction among agents in a mixed competitive game. 
In this project SAC and PPO  algorithms were emoloyed to test and compare peformance. 


## Game Rules 

The game consists of two teams ; Chasers and Builders. 

- The objective of the chasers are to eliminate all the builders with a ball  ( basicaly kick the ball in the direction of the builders in hope that the ball touches the builder). In the even the ball thouches the builder the chaser gets a positive reward and  and the buider gets a negative reward . The episode ends when all the builders have been eliminated and the scene  resets

- The objective of the Builders are to collect all the coins in the scene and place them in the basket. The aim of the builder is to complete this task befoer the chasers have a chance to eliminate their entire team with a ball. The chasers recieve a positive reward propositional to the number of coins they pick up and drop in the basket. upon completion of this task the game resets

Because we awknoweledge that our agents are going to struggle with  knowing what their task is in the beginning  and have a high exploration rate, the game is set to reset after a certain number of steps  . In addition to this both agents are given negative rewards corrosponding to the amount of time they take to complete their rspective task. 



## Training Process 

So its always better to start out as simple as possible, so I first started off with mearly Just traning the builder to collect a certain number of coins and put it in the basket. The agent was trained with PPO and SAC and the results were then compared :
  
  In the sceond case of training for the builder The difficulty was slightly increased by introducing the ball in the scene  in order for the agant to learn to avoide it . 
  
  
  in the third instance I added multiple builders to  observe if when trained together they  would prehaps learn strategies to collect coins  ( to give the agents a sence of preception, reycasts and tag detection  were added to all agents including the chaser). 
  
  In non mixed setting the training process was reasonably simple and straight foward in this case as the task was simple enough. In these cases the agents were able to able to learn their tasks well enough. 
  
  The case of the Chasers turned out to be more challenging as the task of kicking a ball, much less kicking the ball on a moving object proved to be not as straightfoward to learn. 
  In an attempt to achieve good results the Builders were kept stationary and the number of chasers in the scene were increased to 15  while the target amount of builders to hit in order to win was set to 1 then gradually increased.  This experiment was carried out with one as well as multiple chasers in the scene. Ultimately good results were not able to be achieved with this task by both algorithms. The average number of episodes used for each experiment was about 300k. 
  
  ## How to run program 
  
  - The scene is mostly setup, so after you have downloded unity and the mlagents package you can pretty much select the desired scene and train your agent.
  futher info on how to train  agents can be found at https://github.com/Unity-Technologies/ml-agents. In addition to this please download the assets from the last link mentioned in the references and place it in the same folder as the rest of the files . 
  
  
  
  ## references 
  - https://learn.unity.com/project/ml-agents-penguins
  - https://github.com/Unity-Technologies/ml-agents
  ### Assets Used :
   https://assetstore.unity.com/packages/3d/characters/meshtint-free-boximon-fiery-mega-toon-series-153958
   https://assetstore.unity.com/packages/3d/prototyping-pack-free-94277
   
  
