﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.Text;
namespace LUCiD
{
    class Timer
    {
        private int endTimer;
        private int countTimerRef;
        public String displayClock { get; private set; }
        public bool isRunning { get; private set; }
        public bool isFinished { get; private set; }


        public Timer()
        {
            displayClock = "";
            endTimer = 0;
            countTimerRef = 0;
            isRunning = false;
            isFinished = false;

        }
        public void start(int seconds)
        {
            endTimer = seconds;
            isRunning = true;
            displayClock = endTimer.ToString();
        }

        public Boolean checkTime(GameLoop game, GameTime gameTime)
        {
            countTimerRef += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (!isFinished)
            {
                if (countTimerRef >= 1000.0f)
                {
                    endTimer = endTimer - 1;
                    displayClock = endTimer.ToString();
                    countTimerRef = 0;
                    if (endTimer <= 0)
                    {
                        endTimer = 0;
                        isFinished = true;
                        displayClock = "Game Over";
                        game.GameOver();
                    }
                }
            }
            else
            {
                displayClock = "Game Over";
            }
            return isFinished;
        }
        public void reset()
        {
            isRunning = false;
            isFinished = false;
            displayClock = "None";
            countTimerRef = 0;
            endTimer = 0;
        }
    }
} 
