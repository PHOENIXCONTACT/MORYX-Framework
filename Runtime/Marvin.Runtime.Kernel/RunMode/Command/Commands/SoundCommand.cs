using System;
using System.Collections.Generic;
using System.Threading;
using Marvin.Container;
using Marvin.Runtime.Kernel.Additionals;

namespace Marvin.Runtime.Kernel
{
    [Registration(LifeCycle.Singleton, typeof(ICommandHandler))]
    internal class SoundCommand : ICommandHandler
    {
        /// <summary>
        /// Class to keep reference to current player
        /// </summary>
        private class CurrentPlayer
        {
            /// <summary>
            /// Flag if player should keep running
            /// </summary>
            internal bool KeepPlaying { get; set; }

            /// <summary>
            /// Sequence of sounds to play
            /// </summary>
            internal ICollection<SoundGroup> Sequence { get; set; }
        }

        private CurrentPlayer _player;
        private readonly StageCommand _stageCommand = new StageCommand("juke", "box");

        public bool CanHandle(string command)
        {
            // 3 stage input for better hiding
            return _stageCommand.HandleChain(command);
        }

        public void Handle(string[] fullCommand)
        {
            // Stop current player
            if (_player != null)
                _player.KeepPlaying = false;

            // Get matching sequence
            ICollection<SoundGroup> sequence = null;
            switch (fullCommand.Length == 2 ? fullCommand[1] : string.Empty)
            {
                case "empire":
                    sequence = SoundSequences.Empire();
                    break;
                case "tetris":
                    sequence = SoundSequences.Tetris();
                    break;
                case "mario":
                    sequence = SoundSequences.Mario();
                    break;
            }

            // Start new player
            if (sequence != null)
                ThreadPool.QueueUserWorkItem(PlaySequence, (_player = new CurrentPlayer { KeepPlaying = true, Sequence = sequence }));
        }

        public void ExportValidCommands(int pad)
        {

        }

        private void PlaySequence(object playerObject)
        {
            // Global endless loop
            var player = (CurrentPlayer) playerObject;
            while (player.KeepPlaying)
            {
                // Local group loop
                var enumerator = player.Sequence.GetEnumerator();
                while (player.KeepPlaying && enumerator.MoveNext())
                {
                    var group = enumerator.Current;
                    var soundEnumerator = group.Sounds.GetEnumerator();
                    while (player.KeepPlaying && soundEnumerator.MoveNext())
                    {
                        var fragment = soundEnumerator.Current;
                        Console.Beep(fragment.Sound, fragment.Duration);
                    }
                    Thread.Sleep(group.Pause);
                }

            }
        }
    }
}
