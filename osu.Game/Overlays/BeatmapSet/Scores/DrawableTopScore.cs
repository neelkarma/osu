﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Humanizer;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Leaderboards;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osu.Game.Users;
using osuTK;
using osuTK.Graphics;
using System.Collections.Generic;

namespace osu.Game.Overlays.BeatmapSet.Scores
{
    public class DrawableTopScore : Container
    {
        private const float fade_duration = 100;
        private const float height = 100;
        private const float avatar_size = 80;
        private const float margin = 10;

        private OsuColour colours;

        private Color4 backgroundIdleColour => colours.Gray3;
        private Color4 backgroundHoveredColour => colours.Gray4;

        private readonly Box background;
        private readonly UpdateableAvatar avatar;
        private readonly DrawableFlag flag;
        private readonly ClickableTopScoreUsername username;
        private readonly SpriteText rankText;
        private readonly SpriteText date;
        private readonly DrawableRank rank;

        private readonly AutoSizedInfoColumn totalScore;
        private readonly AutoSizedInfoColumn accuracy;
        private readonly MediumInfoColumn maxCombo;

        private readonly SmallInfoColumn hitGreat;
        private readonly SmallInfoColumn hitGood;
        private readonly SmallInfoColumn hitMeh;
        private readonly SmallInfoColumn hitMiss;
        private readonly SmallInfoColumn pp;

        private readonly ModsInfoColumn modsInfo;

        private APIScoreInfo score;
        public APIScoreInfo Score
        {
            get { return score; }
            set
            {
                if (score == value)
                    return;
                score = value;

                avatar.User = username.User = score.User;
                flag.Country = score.User.Country;
                date.Text = $@"achieved {score.Date.Humanize()}";
                rank.UpdateRank(score.Rank);

                totalScore.Value = $@"{score.TotalScore:N0}";
                accuracy.Value = $@"{score.Accuracy:P2}";
                maxCombo.Value = $@"{score.MaxCombo:N0}x";

                hitGreat.Value = $"{score.Statistics[HitResult.Great]}";
                hitGood.Value = $"{score.Statistics[HitResult.Good]}";
                hitMeh.Value = $"{score.Statistics[HitResult.Meh]}";
                hitMiss.Value = $"{score.Statistics[HitResult.Miss]}";
                pp.Value = $@"{score.PP:N0}";

                modsInfo.ClearMods();
                modsInfo.Mods = score.Mods;
            }
        }

        public DrawableTopScore()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            CornerRadius = 10;
            Masking = true;
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Colour = Color4.Black.Opacity(0.2f),
                Radius = 1,
                Offset = new Vector2(0, 1),
            };
            Children = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding(margin),
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(margin, 0),
                            Children = new Drawable[]
                            {
                                rankText = new SpriteText
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Text = "#1",
                                    TextSize = 30,
                                    Font = @"Exo2.0-BoldItalic",
                                },
                                rank = new DrawableRank(ScoreRank.F)
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Size = new Vector2(40),
                                    FillMode = FillMode.Fit,
                                },
                                avatar = new UpdateableAvatar
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Size = new Vector2(avatar_size),
                                    Masking = true,
                                    CornerRadius = 5,
                                    EdgeEffect = new EdgeEffectParameters
                                    {
                                        Type = EdgeEffectType.Shadow,
                                        Colour = Color4.Black.Opacity(0.25f),
                                        Offset = new Vector2(0, 2),
                                        Radius = 1,
                                    },
                                },
                                new FillFlowContainer
                                {
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    AutoSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(0, 3),
                                    Children = new Drawable[]
                                    {
                                        username = new ClickableTopScoreUsername
                                        {
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            TextSize = 20,
                                        },
                                        date = new SpriteText
                                        {
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            TextSize = 15,
                                            Font = @"Exo2.0-Bold",
                                        },
                                        flag = new DrawableFlag
                                        {
                                            Anchor = Anchor.CentreLeft,
                                            Origin = Anchor.CentreLeft,
                                            Size = new Vector2(20, 13),
                                        },
                                    }
                                }
                            }
                        },
                        new Container
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Width = 0.65f,
                            Child = new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Y,
                                RelativeSizeAxes = Axes.X,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Spacing = new Vector2(margin),
                                Children = new Drawable[]
                                {
                                    new FillFlowContainer
                                    {
                                        Anchor = Anchor.TopRight,
                                        Origin = Anchor.TopRight,
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Horizontal,
                                        Spacing = new Vector2(margin, 0),
                                        Children = new Drawable[]
                                        {
                                            hitGreat = new SmallInfoColumn("300", 20),
                                            hitGood = new SmallInfoColumn("100", 20),
                                            hitMeh = new SmallInfoColumn("50", 20),
                                            hitMiss = new SmallInfoColumn("misses", 20),
                                            pp = new SmallInfoColumn("pp", 20),
                                            modsInfo = new ModsInfoColumn("mods"),
                                        }
                                    },
                                    new FillFlowContainer
                                    {
                                        Anchor = Anchor.TopRight,
                                        Origin = Anchor.TopRight,
                                        AutoSizeAxes = Axes.Both,
                                        Direction = FillDirection.Horizontal,
                                        Spacing = new Vector2(margin, 0),
                                        Children = new Drawable[]
                                        {
                                            totalScore = new AutoSizedInfoColumn("Total Score"),
                                            accuracy = new AutoSizedInfoColumn("Accuracy"),
                                            maxCombo = new MediumInfoColumn("Max Combo"),
                                        }
                                    },
                                }
                            }
                        }
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            this.colours = colours;

            rankText.Colour = colours.Yellow;
            background.Colour = backgroundIdleColour;
        }

        protected override bool OnHover(HoverEvent e)
        {
            background.FadeColour(backgroundHoveredColour, fade_duration, Easing.OutQuint);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            background.FadeColour(backgroundIdleColour, fade_duration, Easing.OutQuint);
            base.OnHoverLost(e);
        }

        private class ClickableTopScoreUsername : ClickableUserContainer
        {
            private const float username_fade_duration = 500;

            private readonly Box underscore;
            private readonly Container underscoreContainer;
            private readonly SpriteText text;

            private Color4 hoverColour;

            public float TextSize
            {
                set
                {
                    if (text.TextSize == value)
                        return;
                    text.TextSize = value;
                }
                get { return text.TextSize; }
            }

            public ClickableTopScoreUsername()
            {
                Add(underscoreContainer = new Container
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.X,
                    Height = 1,
                    Child = underscore = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                    }
                });
                Add(text = new SpriteText
                {
                    Font = @"Exo2.0-BoldItalic",
                });
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                hoverColour = underscore.Colour = colours.Blue;
                underscoreContainer.Position = new Vector2(0, TextSize / 2 - 1);
            }

            protected override void OnUserChange(User user)
            {
                text.Text = user.Username;
            }

            protected override bool OnHover(HoverEvent e)
            {
                text.FadeColour(hoverColour, username_fade_duration, Easing.OutQuint);
                underscore.FadeIn(username_fade_duration, Easing.OutQuint);
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                text.FadeColour(Color4.White, username_fade_duration, Easing.OutQuint);
                underscore.FadeOut(username_fade_duration, Easing.OutQuint);
                base.OnHoverLost(e);
            }
        }

        private class DrawableInfoColumn : FillFlowContainer
        {
            private const float header_text_size = 12;

            private readonly Box line;

            protected DrawableInfoColumn(string header)
            {
                AutoSizeAxes = Axes.Y;
                Direction = FillDirection.Vertical;
                Spacing = new Vector2(0, 2);
                Children = new Drawable[]
                {
                    new Container
                    {
                        AutoSizeAxes = Axes.X,
                        Height = header_text_size,
                        Child = new SpriteText
                        {
                            TextSize = 12,
                            Text = header.ToUpper(),
                            Font = @"Exo2.0-Black",
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 2,
                        Child = line = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                        }
                    }
                };
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colours)
            {
                line.Colour = colours.Gray5;
            }
        }

        private class ModsInfoColumn : DrawableInfoColumn
        {
            private readonly FillFlowContainer modsContainer;

            public IEnumerable<Mod> Mods
            {
                set
                {
                    foreach (Mod mod in value)
                        modsContainer.Add(new ModIcon(mod)
                        {
                            AutoSizeAxes = Axes.Both,
                            Scale = new Vector2(0.3f),
                        });
                }
            }

            public ModsInfoColumn(string header) : base(header)
            {
                AutoSizeAxes = Axes.Both;
                Add(modsContainer = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                });
            }

            public void ClearMods() => modsContainer.Clear();
        }

        private class TextInfoColumn : DrawableInfoColumn
        {
            private readonly SpriteText valueText;

            public string Value
            {
                set
                {
                    if (valueText.Text == value)
                        return;
                    valueText.Text = value;
                }
                get { return valueText.Text; }
            }

            protected TextInfoColumn(string header, float valueTextSize = 25) : base(header)
            {
                Add(valueText = new SpriteText
                {
                    TextSize = valueTextSize,
                });
            }
        }

        private class AutoSizedInfoColumn : TextInfoColumn
        {
            public AutoSizedInfoColumn(string header, float valueTextSize = 25) : base(header, valueTextSize)
            {
                AutoSizeAxes = Axes.Both;
            }
        }

        private class MediumInfoColumn : TextInfoColumn
        {
            private const float width = 70;

            public MediumInfoColumn(string header, float valueTextSize = 25) : base(header, valueTextSize)
            {
                Width = width;
            }
        }

        private class SmallInfoColumn : TextInfoColumn
        {
            private const float width = 40;

            public SmallInfoColumn(string header, float valueTextSize = 25) : base(header, valueTextSize)
            {
                Width = width;
            }
        }
    }
}
