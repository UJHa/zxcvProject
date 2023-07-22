namespace Utils
{
    public class UmUtil
    {
        private static int testFrame = 120;
        private static float oneFrameTime = 0f;
        
        private static bool _isSliderHold = false;

        public static float GetOnFrameTime()
        {
            if (0f == oneFrameTime)
                oneFrameTime = 1f / testFrame;
            return oneFrameTime;
        }

        public static void SetSliderHold(bool isHold)
        {
            _isSliderHold = isHold;
        }
        
        public static bool IsSliderHold()
        {
            return _isSliderHold;
        }
    }
}