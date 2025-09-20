using CutTheRope.ios;

namespace CutTheRope.iframework.media;

internal partial class MovieMgr : NSObject
{
	public NSString url;

	public MovieMgrDelegate delegateMovieMgrDelegate;

	public partial void playURL(NSString moviePath, bool mute);

}
