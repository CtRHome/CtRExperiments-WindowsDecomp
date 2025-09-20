using System;
using System.Collections.Generic;
using CutTheRope.ctr_original;
using CutTheRope.iframework.core;
using CutTheRope.ios;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace CutTheRope.iframework.media;

internal class SoundMgr : NSObject
{
	private static ContentManager _contentManager;

	private Dictionary<int, SoundEffect> LoadedSounds;

	private List<SoundEffectInstance> activeSounds;

	private List<SoundEffectInstance> activeLoopedSounds;

	private Song song;

	private int LastID = -1;

	private Dictionary<int, Song> AllSongs = new Dictionary<int, Song>();

	protected static int currentMusicId = -1;

	public static bool gamePlayingMusic = false;

	public new SoundMgr init()
	{
		LoadedSounds = new Dictionary<int, SoundEffect>();
		activeSounds = new List<SoundEffectInstance>();
		activeLoopedSounds = new List<SoundEffectInstance>();
		return this;
	}

	public static void SetContentManager(ContentManager contentManager)
	{
		_contentManager = contentManager;
	}

	public void freeSound(int resId)
	{
		LoadedSounds.Remove(resId);
	}

	public SoundEffect getSound(int resId)
	{
		if (resId == 197 || resId == 198)
		{
			return null;
		}
		if (LoadedSounds.TryGetValue(resId, out var value))
		{
			return value;
		}
		try
		{
			value = _contentManager.Load<SoundEffect>("ctre/sounds/" + CTRResourceMgr.XNA_ResName(resId));
			LoadedSounds.Add(resId, value);
		}
		catch (Exception)
		{
		}
		return value;
	}

	private void ClearStopped()
	{
		List<SoundEffectInstance> list = new List<SoundEffectInstance>();
		foreach (SoundEffectInstance activeSound in activeSounds)
		{
			if (activeSound != null && activeSound.State != SoundState.Stopped)
			{
				list.Add(activeSound);
			}
		}
		activeSounds.Clear();
		activeSounds = list;
	}

	public virtual void playSound(int sid)
	{
		ClearStopped();
		activeSounds.Add(play(sid, l: false));
	}

	public virtual SoundEffectInstance playSoundLooped(int sid)
	{
		ClearStopped();
		SoundEffectInstance soundEffectInstance = play(sid, l: true);
		activeLoopedSounds.Add(soundEffectInstance);
		return soundEffectInstance;
	}

	public virtual void playMusic(int sid)
	{
		if (MediaPlayer.GameHasControl)
		{
			if (!AllSongs.TryGetValue(sid, out song))
			{
				song = _contentManager.Load<Song>("ctre/sounds/" + CTRResourceMgr.XNA_ResName(sid));
				AllSongs.Add(sid, song);
			}
			MediaPlayer.IsRepeating = true;
			if (LastID != sid || MediaPlayer.State == MediaState.Stopped)
			{
				MediaPlayer.Play(song);
				LastID = sid;
			}
			else
			{
				MediaPlayer.Resume();
			}
			gamePlayingMusic = true;
		}
	}

	public void LoadMusic(int sid)
	{
		if (!AllSongs.TryGetValue(sid, out song))
		{
			song = _contentManager.Load<Song>("ctre/sounds/" + CTRResourceMgr.XNA_ResName(sid));
			AllSongs.Add(sid, song);
		}
		song = null;
	}

	public virtual void stopLoopedSounds()
	{
		stopList(activeLoopedSounds);
		activeLoopedSounds.Clear();
	}

	public virtual void stopAllSounds()
	{
		stopLoopedSounds();
	}

	public virtual void stopMusic()
	{
		gamePlayingMusic = false;
		if (MediaPlayer.GameHasControl)
		{
			MediaPlayer.Stop();
		}
	}

	public virtual void suspend()
	{
	}

	public virtual void resume()
	{
	}

	public virtual void pause()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		try
		{
			changeListState(activeLoopedSounds, SoundState.Playing, SoundState.Paused);
			if (MediaPlayer.GameHasControl && (int)MediaPlayer.State == 1)
			{
				MediaPlayer.Pause();
			}
		}
		catch (Exception)
		{
		}
	}

	public virtual void unpause()
	{
		try
		{
			changeListState(activeLoopedSounds, SoundState.Paused, SoundState.Playing);
			if (Preferences._getBooleanForKey("MUSIC_ON") && currentMusicId != -1 && (gamePlayingMusic || MediaPlayer.GameHasControl))
			{
				playMusic(currentMusicId);
			}
		}
		catch (Exception)
		{
		}
	}

	private SoundEffectInstance play(int sid, bool l)
	{
		SoundEffectInstance soundEffectInstance = null;
		try
		{
			soundEffectInstance = getSound(sid).CreateInstance();
			soundEffectInstance.IsLooped = l;
			soundEffectInstance.Play();
		}
		catch (Exception)
		{
		}
		return soundEffectInstance;
	}

	private static void stopList(List<SoundEffectInstance> list)
	{
		foreach (SoundEffectInstance item in list)
		{
			item?.Stop();
		}
	}

	private static void changeListState(List<SoundEffectInstance> list, SoundState fromState, SoundState toState)
	{
		foreach (SoundEffectInstance item in list)
		{
			if (item != null && item.State == fromState)
			{
				switch (toState)
				{
				case SoundState.Playing:
					item.Resume();
					break;
				case SoundState.Paused:
					item.Pause();
					break;
				}
			}
		}
	}
}
