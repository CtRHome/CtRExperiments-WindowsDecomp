using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using CutTheRope.ios;

namespace CutTheRope.iframework.core;

internal partial class Preferences : NSObject
{
	private static Dictionary<string, int> data_ = new Dictionary<string, int>();

	private static Dictionary<string, string> dataStrings_ = new Dictionary<string, string>();

	private static string saveFileName_ = "ctr.sav";

	public static bool firstStart = true;

	public override NSObject init()
	{
		if (base.init() == null)
		{
			return null;
		}
		_loadPreferences();
		return this;
	}

	public virtual void setIntforKey(int v, string k, bool comit)
	{
		_setIntforKey(v, k, comit);
	}

	public virtual void setBooleanforKey(bool v, string k, bool comit)
	{
		_setBooleanforKey(v, k, comit);
	}

	public virtual void setStringforKey(string v, string k, bool comit)
	{
		_setStringforKey(v, k, comit);
	}

	public virtual int getIntForKey(string k)
	{
		return _getIntForKey(k);
	}

	public virtual float getFloatForKey(string k)
	{
		return 0f;
	}

	public virtual bool getBooleanForKey(string k)
	{
		return _getBooleanForKey(k);
	}

	public virtual string getStringForKey(string k)
	{
		return _getStringForKey(k);
	}

	public static void _setIntforKey(int v, string key, bool comit)
	{
		if (data_.TryGetValue(key, out var _))
		{
			data_[key] = v;
		}
		else
		{
			data_.Add(key, v);
		}
		if (comit)
		{
			_savePreferences();
		}
	}

	public static void _setStringforKey(string v, string k, bool comit)
	{
		if (dataStrings_.TryGetValue(k, out var _))
		{
			dataStrings_[k] = v;
		}
		else
		{
			dataStrings_.Add(k, v);
		}
		if (comit)
		{
			_savePreferences();
		}
	}

	public static int _getIntForKey(string k)
	{
		if (data_.TryGetValue(k, out var value))
		{
			return value;
		}
		return 0;
	}

	private static float _getFloatForKey(string k)
	{
		return 0f;
	}

	public static bool _getBooleanForKey(string k)
	{
		int num = _getIntForKey(k);
		return num != 0;
	}

	public static void _setBooleanforKey(bool v, string k, bool comit)
	{
		_setIntforKey(v ? 1 : 0, k, comit);
	}

	public static string _getStringForKey(string k)
	{
		if (dataStrings_.TryGetValue(k, out var value))
		{
			return value;
		}
		return "";
	}

    // in mobile platforms it should use IsolatedStorageFile

    private static partial Stream CreateFile(string name);

	private static partial Stream OpenFile(string name, FileMode mode);

	private static partial void DeleteFile(string name);

	private static partial void MoveFile(string from, string to);

	private static partial bool FileExists(string name);

	public virtual void savePreferences()
	{
		_savePreferences();
	}

	public static void _savePreferences()
	{
		string text = saveFileName_ + ".bak";
		bool flag = false;
		bool flag2 = false;
		try
		{
			try
			{
				DeleteFile(text);
				MoveFile(saveFileName_, text);
				flag = true;
			}
			catch (Exception)
			{
			}
			using Stream output = CreateFile(saveFileName_);
			BinaryWriter binaryWriter = new BinaryWriter(output);
			binaryWriter.Write(data_.Count);
			foreach (KeyValuePair<string, int> item in data_)
			{
				binaryWriter.Write(item.Key);
				binaryWriter.Write(item.Value);
			}
			binaryWriter.Write(dataStrings_.Count);
			foreach (KeyValuePair<string, string> item2 in dataStrings_)
			{
				binaryWriter.Write(item2.Key);
				binaryWriter.Write(item2.Value);
			}
			binaryWriter.Close();
			flag2 = true;
		}
		catch (Exception ex2)
		{
			FrameworkTypes._LOG("Error: cannot save, " + ex2.ToString());
		}
		if (flag2 || !flag)
		{
			return;
		}
		try
		{
			DeleteFile(saveFileName_);
			MoveFile(text, saveFileName_);
		}
		catch (Exception)
		{
		}
	}

	public virtual bool loadPreferences()
	{
		return _loadPreferences();
	}

	internal static bool _loadPreferences()
	{
        if (FileExists(saveFileName_))
        {
            try
            {
                using Stream input = OpenFile(saveFileName_, FileMode.Open);
                BinaryReader binaryReader = new BinaryReader(input);
                int num = binaryReader.ReadInt32();
                for (int i = 0; i < num; i++)
                {
                    string key = binaryReader.ReadString();
                    int value = binaryReader.ReadInt32();
                    data_.Add(key, value);
                }
                num = binaryReader.ReadInt32();
                for (int j = 0; j < num; j++)
                {
                    string key2 = binaryReader.ReadString();
                    string value2 = binaryReader.ReadString();
                    dataStrings_.Add(key2, value2);
                }
                firstStart = false;
                binaryReader.Close();
            }
            catch (Exception ex)
            {
                FrameworkTypes._LOG("Error: cannot load, " + ex.ToString());
            }
        }
        return !firstStart;
	}
}
