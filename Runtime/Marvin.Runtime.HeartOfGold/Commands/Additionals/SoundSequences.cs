using System.Collections.Generic;

namespace Marvin.Runtime.HeartOfGold
{
    /// <summary>
    /// Fragment representing a single beep sound
    /// </summary>
    internal class SoundFragment
    {
        public SoundFragment(int sound, int duration)
        {
            Sound = sound;
            Duration = duration;
        }

        /// <summary>
        /// Sound to play
        /// </summary>
        internal int Sound { get; private set; }

        /// <summary>
        /// Duration of sound
        /// </summary>
        internal int Duration { get; private set; }
    }

    /// <summary>
    /// Group of sounds followed by pause
    /// </summary>
    internal class SoundGroup
    {
        /// <summary>
        /// A couple of sounds
        /// </summary>
        internal ICollection<SoundFragment> Sounds { get; set; }

        /// <summary>
        /// Pause in milliseconds before next group
        /// </summary>
        internal int Pause { get; set; }
    }

    internal class SoundSequences
    { 
        internal static ICollection<SoundGroup> Empire()
        {
            var groups = new List<SoundGroup>
            {
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(440, 500), new SoundFragment(440, 500), new SoundFragment(440, 500),
                        new SoundFragment(349, 350), new SoundFragment(523, 150), new SoundFragment(440, 500),
                        new SoundFragment(349, 350), new SoundFragment(523, 150), new SoundFragment(440, 650)
                    },
                    Pause = 150,
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(659, 500), new SoundFragment(659, 500), new SoundFragment(659, 500),
                        new SoundFragment(698, 350), new SoundFragment(523, 150), new SoundFragment(415, 500),
                        new SoundFragment(349, 350), new SoundFragment(523, 150), new SoundFragment(440, 650)
                    },
                    Pause = 150
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(880, 500), new SoundFragment(440, 300), new SoundFragment(440, 150),
                        new SoundFragment(880, 400), new SoundFragment(830, 200), new SoundFragment(784, 200),
                        new SoundFragment(740, 125), new SoundFragment(698, 125), new SoundFragment(740, 250)
                    },
                    Pause = 250
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(455, 250), new SoundFragment(622, 400), new SoundFragment(587, 200),
                        new SoundFragment(554, 200), new SoundFragment(523, 125), new SoundFragment(466, 125),
                        new SoundFragment(523, 250)
                    },
                    Pause = 250
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(349, 125), new SoundFragment(415, 500), new SoundFragment(349, 375),
                        new SoundFragment(440, 125), new SoundFragment(523, 500), new SoundFragment(440, 375),
                        new SoundFragment(523, 125), new SoundFragment(659, 650), new SoundFragment(880, 500),
                        new SoundFragment(440, 300), new SoundFragment(440, 150), new SoundFragment(880, 400),
                        new SoundFragment(830, 200), new SoundFragment(784, 200), new SoundFragment(740, 125),
                        new SoundFragment(698, 125), new SoundFragment(740, 250)
                    },
                    Pause = 250
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(255, 250), new SoundFragment(622, 400), new SoundFragment(587, 200),
                        new SoundFragment(455, 250), new SoundFragment(622, 400), new SoundFragment(787, 200),
                        new SoundFragment(544, 200), new SoundFragment(523, 125), new SoundFragment(466, 125),
                        new SoundFragment(523, 250)
                    },
                    Pause = 250
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(349, 250), new SoundFragment(415, 500), new SoundFragment(349, 375),
                        new SoundFragment(523, 125), new SoundFragment(440, 500), new SoundFragment(349, 375),
                        new SoundFragment(523, 125), new SoundFragment(440, 650)
                    },
                    Pause = 1000
                }
            };
            return groups;  
        }

        internal static ICollection<SoundGroup> Tetris()
        {
            var groups = new List<SoundGroup>
            {
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(1320, 500), new SoundFragment(990, 250),  new SoundFragment(1056, 250),
                        new SoundFragment(1188, 250), new SoundFragment(1320, 125), new SoundFragment(1188, 125),
                        new SoundFragment(1056, 250), new SoundFragment(990, 250),  new SoundFragment(880, 500),
                        new SoundFragment(880, 250),  new SoundFragment(1056, 250), new SoundFragment(1320, 500),
                        new SoundFragment(1188, 250), new SoundFragment(1056, 250), new SoundFragment(990, 750),
                        new SoundFragment(1056, 250), new SoundFragment(1188, 500), new SoundFragment(1320, 500),
                        new SoundFragment(1056, 500), new SoundFragment(880, 500),  new SoundFragment(880, 500)
                    },
                    Pause = 250
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(1188, 500), new SoundFragment(1408, 250), new SoundFragment(1760, 500),
                        new SoundFragment(1584, 250), new SoundFragment(1408, 250), new SoundFragment(1320, 750),
                        new SoundFragment(1056, 250), new SoundFragment(1320, 500), new SoundFragment(1188, 250),
                        new SoundFragment(1056, 250), new SoundFragment(990, 500),  new SoundFragment(990, 250),
                        new SoundFragment(1056, 250), new SoundFragment(1188, 500), new SoundFragment(1320, 500),
                        new SoundFragment(1056, 500), new SoundFragment(880, 500),  new SoundFragment(880, 500)
                    },
                    Pause = 500
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(1320, 500), new SoundFragment(990, 250),  new SoundFragment(1056, 250),
                        new SoundFragment(1188, 250), new SoundFragment(1320, 125), new SoundFragment(1188, 125),
                        new SoundFragment(1056, 250), new SoundFragment(990, 250),  new SoundFragment(880, 500),
                        new SoundFragment(880, 250),  new SoundFragment(1056, 250), new SoundFragment(1320, 500), 
                        new SoundFragment(1188, 250), new SoundFragment(1056, 250), new SoundFragment(990, 750),
                        new SoundFragment(1056, 250), new SoundFragment(1188, 500), new SoundFragment(1320, 500),
                        new SoundFragment(1056, 500), new SoundFragment(880, 500),  new SoundFragment(880, 500)
                    },
                    Pause = 250
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(1188, 500), new SoundFragment(1408, 250), new SoundFragment(1760, 500),
                        new SoundFragment(1584, 250), new SoundFragment(1408, 250), new SoundFragment(1320, 750),
                        new SoundFragment(1056, 250), new SoundFragment(1320, 500), new SoundFragment(1188, 250),
                        new SoundFragment(1056, 250), new SoundFragment(990, 500),  new SoundFragment(990, 250),
                        new SoundFragment(1056, 250), new SoundFragment(1188, 500), new SoundFragment(1320, 500),
                        new SoundFragment(1056, 500), new SoundFragment(880, 500),  new SoundFragment(880, 500),
                    },
                    Pause = 500
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(660, 1000), new SoundFragment(528, 1000), new SoundFragment(594, 1000),
                        new SoundFragment(495, 1000), new SoundFragment(528, 1000), new SoundFragment(440, 1000),
                        new SoundFragment(419, 1000), new SoundFragment(495, 1000), new SoundFragment(660, 1000),
                        new SoundFragment(528, 1000), new SoundFragment(594, 1000), new SoundFragment(495, 1000),
                        new SoundFragment(528, 500),  new SoundFragment(660, 500),  new SoundFragment(880, 1000),
                        new SoundFragment(838, 2000), new SoundFragment(660, 1000), new SoundFragment(528, 1000),
                        new SoundFragment(594, 1000), new SoundFragment(495, 1000), new SoundFragment(528, 1000),
                        new SoundFragment(440, 1000), new SoundFragment(419, 1000), new SoundFragment(495, 1000),
                        new SoundFragment(660, 1000), new SoundFragment(528, 1000), new SoundFragment(594, 1000),
                        new SoundFragment(495, 1000), new SoundFragment(528, 500),  new SoundFragment(660, 500),
                        new SoundFragment(880, 1000), new SoundFragment(838, 2000)
                    },
                    Pause = 1000
                }
            };
            return groups;
        }

        internal static ICollection<SoundGroup> Mario()
        {
            //;###################################################
            //; Mario Bros Theme ( Beep Music )###################
            //;###################################################
            //;###################### by J0keR  ##################
            //;###################################################
            var groups = new List<SoundGroup>
            {
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(480, 200), new SoundFragment(1568, 200), new SoundFragment(1568, 200),
                        new SoundFragment(1568, 200), new SoundFragment(740, 200), new SoundFragment(784, 200),
                        new SoundFragment(784, 200), new SoundFragment(784, 200), new SoundFragment(370, 200),
                        new SoundFragment(392, 200), new SoundFragment(370, 200), new SoundFragment(392, 200),
                        new SoundFragment(392, 400), new SoundFragment(196, 400), new SoundFragment(740, 200),
                        new SoundFragment(784, 200), new SoundFragment(784, 200), new SoundFragment(740, 200),
                        new SoundFragment(784, 200), new SoundFragment(784, 200), new SoundFragment(740, 200),
                        new SoundFragment(84, 200), new SoundFragment(880, 200), new SoundFragment(831, 200),
                        new SoundFragment(880, 200), new SoundFragment(988, 400), new SoundFragment(880, 200),
                        new SoundFragment(784, 200), new SoundFragment(698, 200), new SoundFragment(740, 200),
                        new SoundFragment(784, 200), new SoundFragment(784, 200), new SoundFragment(740, 200),
                        new SoundFragment(784, 200), new SoundFragment(784, 200), new SoundFragment(740, 200),
                        new SoundFragment(784, 200), new SoundFragment(880, 200), new SoundFragment(831, 200),
                        new SoundFragment(880, 200), new SoundFragment(988, 400)
                    },
                    Pause = 200
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(1108, 10), new SoundFragment(1154, 200), new SoundFragment(1480, 10),
                        new SoundFragment(1568, 200)
                    },
                    Pause = 200
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(740, 200), new SoundFragment(784, 200), new SoundFragment(784, 200),
                        new SoundFragment(740, 200), new SoundFragment(784, 200), new SoundFragment(784, 200),
                        new SoundFragment(740, 200), new SoundFragment(784, 200), new SoundFragment(880, 200),
                        new SoundFragment(831, 200), new SoundFragment(880, 200), new SoundFragment(988, 400),
                        new SoundFragment(880, 200), new SoundFragment(784, 200), new SoundFragment(698, 200),
                        new SoundFragment(659, 200), new SoundFragment(698, 200), new SoundFragment(784, 200),
                        new SoundFragment(880, 400), new SoundFragment(784, 200), new SoundFragment(698, 200),
                        new SoundFragment(659, 200), new SoundFragment(587, 200), new SoundFragment(659, 200),
                        new SoundFragment(698, 200), new SoundFragment(784, 400), new SoundFragment(698, 200),
                        new SoundFragment(659, 200), new SoundFragment(587, 200), new SoundFragment(523, 200),
                        new SoundFragment(587, 200), new SoundFragment(659, 200), new SoundFragment(698, 400),
                        new SoundFragment(659, 200), new SoundFragment(587, 200), new SoundFragment(494, 200),
                        new SoundFragment(523, 200)
                    },
                    Pause = 400
                },
                new SoundGroup
                {
                    Sounds = new List<SoundFragment>
                    {
                        new SoundFragment(349, 400), new SoundFragment(392, 200), new SoundFragment(330, 200),
                        new SoundFragment(523, 200), new SoundFragment(494, 200), new SoundFragment(466, 200),
                        new SoundFragment(440, 200), new SoundFragment(494, 200), new SoundFragment(523, 200),
                        new SoundFragment(880, 200), new SoundFragment(494, 200), new SoundFragment(880, 200),
                        new SoundFragment(1760, 200), new SoundFragment(440, 200), new SoundFragment(392, 200),
                        new SoundFragment(440, 200), new SoundFragment(494, 200), new SoundFragment(784, 200),
                        new SoundFragment(440, 200), new SoundFragment(784, 200), new SoundFragment(1568, 200),
                        new SoundFragment(392, 200), new SoundFragment(349, 200), new SoundFragment(392, 200),
                        new SoundFragment(440, 200), new SoundFragment(698, 200), new SoundFragment(415, 200),
                        new SoundFragment(698, 200), new SoundFragment(1397, 200), new SoundFragment(349, 200),
                        new SoundFragment(330, 200), new SoundFragment(311, 200), new SoundFragment(330, 200),
                        new SoundFragment(659, 200), new SoundFragment(698, 400), new SoundFragment(784, 400),
                        new SoundFragment(440, 200), new SoundFragment(494, 200), new SoundFragment(523, 200),
                        new SoundFragment(880, 200), new SoundFragment(494, 200), new SoundFragment(880, 200),
                        new SoundFragment(1760, 200), new SoundFragment(440, 200), new SoundFragment(392, 200),
                        new SoundFragment(440, 200), new SoundFragment(494, 200), new SoundFragment(784, 200),
                        new SoundFragment(440, 200), new SoundFragment(784, 200), new SoundFragment(1568, 200),
                        new SoundFragment(392, 200), new SoundFragment(349, 200), new SoundFragment(392, 200),
                        new SoundFragment(440, 200), new SoundFragment(698, 200), new SoundFragment(659, 200),
                        new SoundFragment(698, 200), new SoundFragment(740, 200), new SoundFragment(784, 200),
                        new SoundFragment(392, 200), new SoundFragment(392, 200), new SoundFragment(392, 200),
                        new SoundFragment(392, 200), new SoundFragment(196, 200), new SoundFragment(196, 200),
                        new SoundFragment(196, 200), new SoundFragment(185, 200), new SoundFragment(196, 200),
                        new SoundFragment(185, 200), new SoundFragment(196, 200), new SoundFragment(208, 200),
                        new SoundFragment(220, 200), new SoundFragment(233, 200), new SoundFragment(247, 200)
                    },
                    Pause = 1000
                }
            };
            return groups;
        }
    }
}
