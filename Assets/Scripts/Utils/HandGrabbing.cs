using Oculus.Interaction.Input;

namespace Utils
{
    public class HandGrabbing
    {
        public static bool IsGrabbing(IHand hand)
        {
            return hand.GetFingerIsPinching(HandFinger.Index) &&
                   hand.GetFingerIsPinching(HandFinger.Middle);
        }

        public static bool IsPinching(IHand hand)
        {
            return hand.GetFingerIsPinching(HandFinger.Index) &&
                   !hand.GetFingerIsPinching(HandFinger.Middle) &&
                   !hand.GetFingerIsPinching(HandFinger.Ring) &&
                   !hand.GetFingerIsPinching(HandFinger.Pinky);
        }
    }
}