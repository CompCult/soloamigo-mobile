//
//  MediaCapture.h
//  Unity-iPhone
//
//  Created by Wili on 2018/1/6.
//

#import <UIKit/UIKit.h>
#import <AVKit/AVKit.h>
#import <AVFoundation/AVFoundation.h>
 #import <MediaPlayer/MediaPlayer.h>
@interface MediaCapture : NSObject<UIImagePickerControllerDelegate, UINavigationControllerDelegate>

// UnityGLViewController keeps this instance.
@property(nonatomic) UIImagePickerController* captureController;

@property(nonatomic) NSString *outputFileName;

@property(nonatomic) BOOL isNeedEdit;
@property(nonatomic) int mediaMode;
+ (instancetype)sharedInstance;

//- (void)show:(NSString *)title outputFileName:(NSString *)name maxSize:(NSInteger)maxSize;
- (void)TakePhoto:(NSString *)name;
- (void)PickPhoto;
- (void)PickVideo;
- (void)CaptureVideo;
- (void)PlayVideo;

@end
