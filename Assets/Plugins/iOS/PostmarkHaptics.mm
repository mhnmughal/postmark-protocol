// Original project source, 2026-08-26. Uses only Apple's built-in UIKit.
#import <UIKit/UIKit.h>

extern "C" void PostmarkLightHaptic()
{
    if (@available(iOS 13.0, *))
    {
        static UIImpactFeedbackGenerator *feedback = nil;
        if (feedback == nil)
            feedback = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
        [feedback prepare];
        [feedback impactOccurredWithIntensity:0.45];
    }
}
