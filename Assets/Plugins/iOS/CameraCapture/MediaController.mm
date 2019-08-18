//
//  ImageController.m
//  Unity-iPhone
//
//  Created by Wili on 2018/1/6.
//

#import <Foundation/Foundation.h>
#import "MediaCapture.h"

extern "C" {
    
    
    
    void iTakePhoto()
    {
        MediaCapture *capturer = [MediaCapture sharedInstance];
        [capturer TakePhoto:[NSString stringWithUTF8String:"output"]];
    }
    
    void iPlayVideo()
    {
        MediaCapture *capturer = [MediaCapture sharedInstance];
        [capturer PlayVideo ];
    }
    
    void iPickPhoto()
    {
        MediaCapture *capturer = [MediaCapture sharedInstance];
        [capturer PickPhoto ];
    }
    
    void iPickVideo()
    {
        MediaCapture *capturer = [MediaCapture sharedInstance];
        [capturer  PickVideo ];
    }
    
    
    void iCaptureVideo()
    {
        MediaCapture *capturer = [MediaCapture sharedInstance];
        [capturer CaptureVideo ];
    }
    
    
}
