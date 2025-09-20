using CutTheRope.ios;
using CutTheRope.utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutTheRope.iframework.media;

internal partial class MovieMgr
{
    private VideoPlayer player;

    private Video video;

    private bool waitForStart;

    private bool paused;

    private bool playing;

    public partial void playURL(NSString moviePath, bool mute)
    {
        url = moviePath;
        if (Global.ScreenSizeManager.CurrentSize.Width <= 1024)
        {
            video = Global.XnaGame.Content.Load<Video>("video/" + moviePath);
        }
        else
        {
            video = Global.XnaGame.Content.Load<Video>("video_hd/" + moviePath);
        }
        player ??= new VideoPlayer();
        player.IsLooped = false;
        player.IsMuted = mute;
        waitForStart = true;
    }

    public Texture2D getTexture()
    {
        if (player != null && player.State != 0)
        {
            return player.GetTexture();
        }
        return null;
    }

    public bool isPlaying()
    {
        return playing;
    }

    public void stop()
    {
        if (player != null)
        {
            player.Stop();
        }
    }

    public void pause()
    {
        if (!paused)
        {
            paused = true;
            if (player != null)
            {
                player.Pause();
            }
        }
    }

    public bool isPaused()
    {
        return paused;
    }

    public void resume()
    {
        if (paused)
        {
            paused = false;
            if (player != null && player.State == MediaState.Paused)
            {
                player.Resume();
            }
        }
    }

    public void start()
    {
        if (waitForStart && player != null && player.State == MediaState.Stopped)
        {
            waitForStart = false;
            playing = true;
            player.Play(video);
        }
    }

    public void update()
    {
        if (!waitForStart && player != null && playing && player.State == MediaState.Stopped)
        {
            video.Dispose();
            video = null;
            paused = false;
            playing = false;
            if (delegateMovieMgrDelegate != null)
            {
                delegateMovieMgrDelegate.moviePlaybackFinished(url);
            }
        }
    }
}