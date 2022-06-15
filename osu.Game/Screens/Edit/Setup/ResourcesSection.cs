// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.IO;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Overlays;

namespace osu.Game.Screens.Edit.Setup
{
    internal class ResourcesSection : SetupSection
    {
        private LabelledFileChooser audioTrackChooser;
        private LabelledFileChooser backgroundChooser;

        public override LocalisableString Title => "Resources";

        [Resolved]
        private MusicController music { get; set; }

        [Resolved]
        private BeatmapManager beatmaps { get; set; }

        [Resolved]
        private IBindable<WorkingBeatmap> working { get; set; }

        [Resolved(canBeNull: true)]
        private Editor editor { get; set; }

        [Resolved]
        private SetupScreenHeader header { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                backgroundChooser = new LabelledFileChooser(".jpg", ".jpeg", ".png")
                {
                    Label = "Background",
                    FixedLabelWidth = LABEL_WIDTH,
                    TabbableContentContainer = this
                },
                audioTrackChooser = new LabelledFileChooser(".mp3", ".ogg")
                {
                    Label = "Audio Track",
                    FixedLabelWidth = LABEL_WIDTH,
                    TabbableContentContainer = this
                },
            };

            if (!string.IsNullOrEmpty(working.Value.Metadata.BackgroundFile))
                backgroundChooser.Current.Value = new FileInfo(working.Value.Metadata.BackgroundFile);

            if (!string.IsNullOrEmpty(working.Value.Metadata.AudioFile))
                audioTrackChooser.Current.Value = new FileInfo(working.Value.Metadata.AudioFile);

            backgroundChooser.Current.BindValueChanged(backgroundChanged, true);
            audioTrackChooser.Current.BindValueChanged(audioTrackChanged, true);
        }

        public bool ChangeBackgroundImage(FileInfo source)
        {
            if (!source.Exists)
                return false;

            var set = working.Value.BeatmapSetInfo;

            var destination = new FileInfo($@"bg{source.Extension}");

            // remove the previous background for now.
            // in the future we probably want to check if this is being used elsewhere (other difficulties?)
            var oldFile = set.Files.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Filename) == destination.Name);

            using (var stream = source.OpenRead())
            {
                if (oldFile != null)
                    beatmaps.DeleteFile(set, oldFile);

                beatmaps.AddFile(set, stream, destination.Name);
            }

            working.Value.Metadata.BackgroundFile = destination.Name;
            header.Background.UpdateBackground();

            return true;
        }

        public bool ChangeAudioTrack(FileInfo source)
        {
            if (!source.Exists)
                return false;

            var set = working.Value.BeatmapSetInfo;

            var destination = new FileInfo($@"audio{source.Extension}");

            // remove the previous audio track for now.
            // in the future we probably want to check if this is being used elsewhere (other difficulties?)
            var oldFile = set.Files.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f.Filename) == destination.Name);

            using (var stream = source.OpenRead())
            {
                if (oldFile != null)
                    beatmaps.DeleteFile(set, oldFile);

                beatmaps.AddFile(set, stream, destination.Name);
            }

            working.Value.Metadata.AudioFile = destination.Name;

            music.ReloadCurrentTrack();

            editor?.UpdateClockSource();
            return true;
        }

        private void backgroundChanged(ValueChangedEvent<FileInfo> file)
        {
            backgroundChooser.Text = file.NewValue == null
                ? "Click to select a background image"
                : "Click to replace the background image";

            if (file.NewValue != file.OldValue)
            {
                if (!ChangeBackgroundImage(file.NewValue))
                    backgroundChooser.Current.Value = file.OldValue;
            }
        }

        private void audioTrackChanged(ValueChangedEvent<FileInfo> file)
        {
            audioTrackChooser.Text = file.NewValue == null
                ? "Click to select a track"
                : "Click to replace the track";

            if (file.NewValue != file.OldValue)
            {
                if (!ChangeAudioTrack(file.NewValue))
                    audioTrackChooser.Current.Value = file.OldValue;
            }
        }
    }
}
