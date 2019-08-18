//
//  MediaCapture.mm
//  Unity-iPhone
//
//  Created by Wili on 2018/1/6.
//

#import "MediaCapture.h"

#pragma mark Config

const char* CALLBACK_OBJECT = "CameraCapture";
const char* CALLBACK_TAKEPHOTO_METHOD = "OnTakePhotoComplete";
const char* CALLBACK_CAPTURE_VIDEO_METHOD = "OnCaptureVideoComplete";
const char* CALLBACK_PICK_METHOD = "OnPickComplete";
const char* CALLBACK_METHOD_FAILURE = "OnFailure";

const char* MESSAGE_FAILED_CAPTURE = "get media data error !";
const char* MESSAGE_FAILED_FIND = "get media data error !";
const char* MESSAGE_FAILED_COPY = "get media data error !";

#pragma mark Picker

@implementation MediaCapture

+ (instancetype)sharedInstance {
    static MediaCapture *instance;
    static dispatch_once_t token;
    dispatch_once(&token, ^{
        instance = [[MediaCapture alloc] init];
    });
    return instance;
}

- (void)TakePhoto:(NSString *)name {
    if (self.captureController != nil) {
           UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_CAPTURE);
        return;
    }
    
    if (![UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera]) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_CAPTURE);
        // don't support open camera
        return ;
    }
    
    self.captureController = [[UIImagePickerController alloc] init];
    self.captureController.delegate = self;
    
    self.captureController.sourceType = UIImagePickerControllerSourceTypeCamera;
    self.mediaMode = 1;
    UIViewController *unityController = UnityGetGLViewController();
    [unityController presentViewController:self.captureController animated:YES completion:^{
        self.outputFileName = [[ MediaCapture getCurrentTimes] stringByAppendingString:@"_output"];
    }];
}





- (void)CaptureVideo{
    if (self.captureController != nil) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_CAPTURE);
        return;
    }
    
    if (![UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera]) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_CAPTURE);
        // don't support open camera
        return ;
    }
    
    
    self.captureController = [[UIImagePickerController alloc] init];
    self.captureController.delegate = self;
    
    self.captureController.allowsEditing = YES;
    self.mediaMode = 3;
    if ([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera]) {
        self.captureController.sourceType = UIImagePickerControllerSourceTypeCamera;
        self.captureController.mediaTypes = [UIImagePickerController availableMediaTypesForSourceType:UIImagePickerControllerSourceTypeCamera];
        self.captureController.videoQuality = UIImagePickerControllerQualityTypeHigh; //set the video quality
        self.captureController.cameraCaptureMode = UIImagePickerControllerCameraCaptureModeVideo; //
        self.captureController.videoMaximumDuration = 600.0f; //set the max record time
        self.captureController.mediaTypes = [NSArray arrayWithObjects:@"public.movie", nil];
    }
    
    UIViewController *unityController = UnityGetGLViewController();
    [unityController presentViewController:self.captureController animated:YES completion:^{
          self.outputFileName = [[ MediaCapture getCurrentTimes] stringByAppendingString:@"_output"];
    }];
    
}


- (void)PlayVideo{
    
    
    NSLog(@"Pick Video from Ablum ! ");
    self.captureController=[[UIImagePickerController alloc] init];
    
    self.captureController.delegate=self;
    
    //media type：@"public.movie" is video  @"public.image"  is picture
    //we only need to show the videos
    self.captureController.mediaTypes = [NSArray arrayWithObjects:@"public.movie", nil ,nil];
    
    self.captureController.sourceType= UIImagePickerControllerSourceTypePhotoLibrary;
    self.mediaMode = 4;
    
    UIViewController *unityController = UnityGetGLViewController();
    [unityController presentViewController:self.captureController animated:YES completion:^{
        self.outputFileName = [[ MediaCapture getCurrentTimes] stringByAppendingString:@"_output.move"];
    }];
    
}

- (void)PickPhoto
{
    if (self.captureController != nil) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_CAPTURE);
        return;
    }
    
    self.captureController = [[UIImagePickerController alloc] init];
    self.captureController.delegate = self;
    
    self.captureController.mediaTypes = [NSArray arrayWithObjects:@"public.image", nil ,nil];
    
    self.captureController.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
     self.mediaMode = 2;
    UIViewController *unityController = UnityGetGLViewController();
    [unityController presentViewController:self.captureController animated:YES completion:^{
        self.outputFileName = [[ MediaCapture getCurrentTimes] stringByAppendingString:@"_output"];
    }];
}

- (void)PickVideo
{
    if (self.captureController != nil) {
        UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_CAPTURE);
        return;
    }
    
    self.captureController = [[UIImagePickerController alloc] init];
    self.captureController.delegate = self;
       self.captureController.mediaTypes = [NSArray arrayWithObjects:@"public.movie", nil ,nil];
    
    self.captureController.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    self.mediaMode = 5;
    UIViewController *unityController = UnityGetGLViewController();
    [unityController presentViewController:self.captureController animated:YES completion:^{
       self.outputFileName = [[ MediaCapture getCurrentTimes] stringByAppendingString:@"_output"];
    }];
}

#pragma mark UIImagePickerControllerDelegate

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary<NSString *,id> *)info {
    
    NSString *mediaType = [info objectForKey:UIImagePickerControllerMediaType];
    if ([mediaType isEqualToString:@"public.image"]) {
        UIImage *image;
        
        image = info[UIImagePickerControllerOriginalImage];
        
       // [[NSFileManager defaultManager] moveItemAtPath:filePath toPath:localFilePath error:&error] ;
        //toMaxWidthAndHeight:(CGFloat)maxValue
        
       
        
        if (image == nil) {
            UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_FIND);
            [self dismissPicker];
            return;
        }
        
        image = [self resizeImage:image toMaxWidthAndHeight:1024] ;
        
        if(self.mediaMode == 2|| self.mediaMode == 1)
        {
            image = [self fixOrientation:image];
        }
        
        if(self.mediaMode == 1)
        {
            if ([UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera]) {
                //image save to album
                UIImageWriteToSavedPhotosAlbum(image, nil, nil, nil);
            }
        }
        
        NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
        if (paths.count == 0) {
            UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_COPY);
            [self dismissPicker];
            return;
        }
        
        NSString *imageName = self.outputFileName;
        if ([imageName hasSuffix:@".png"] == NO) {
            imageName = [imageName stringByAppendingString:@".png"];
        }
        
        NSString *imageSavePath = [(NSString *)[paths objectAtIndex:0] stringByAppendingPathComponent:imageName];
        NSData *png = UIImagePNGRepresentation(image);
        
        if (png == nil) {
            UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_COPY);
            [self dismissPicker];
            return;
        }
        
        BOOL success = [png writeToFile:imageSavePath atomically:YES];
        if (success == NO) {
            UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_COPY);
            [self dismissPicker];
            return;
        }
        if(self.mediaMode == 1)
        {
              UnitySendMessage(CALLBACK_OBJECT, CALLBACK_TAKEPHOTO_METHOD, [imageSavePath UTF8String]);
        }
        else if(self.mediaMode == 2)
        {
              UnitySendMessage(CALLBACK_OBJECT, CALLBACK_PICK_METHOD, [imageSavePath UTF8String]);
        }
      
           NSLog(imageSavePath);
        [self dismissPicker];
    }
    else if ([mediaType isEqualToString:@"public.movie"])
    {
        if( self.mediaMode == 3)
        {
            NSURL *videoUrl = info[UIImagePickerControllerMediaURL];//get video path
            UISaveVideoAtPathToSavedPhotosAlbum([videoUrl path], self, @selector(video:didFinishSavingWithError:contextInfo:), nil);//save to ablum
            UnitySendMessage(CALLBACK_OBJECT, CALLBACK_CAPTURE_VIDEO_METHOD, [[videoUrl path] UTF8String]);// send the  path to unity controller
            [self dismissPicker];
        }
       else if( self.mediaMode == 4)
       {
     
           NSURL *url = [info objectForKey:UIImagePickerControllerMediaURL];
           // Save video to app document directory
           
           NSString *filePath = [url path];
           NSString *pathExtension = [filePath pathExtension] ;
           NSString *localFilePath ;
           if ([pathExtension length] > 0)
           {
               NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) ;
               NSString *documentsDirectory = [paths objectAtIndex:0];
               
                localFilePath = [documentsDirectory stringByAppendingPathComponent:[NSString stringWithFormat:@"%@", [filePath lastPathComponent]]];
               
               // Method last path component is used here, so that each video saved will get different name.
               
               NSError *error = nil ;
               BOOL res = [[NSFileManager defaultManager] moveItemAtPath:filePath toPath:localFilePath error:&error] ;
               
               if (!res)
               {
                   NSLog(@"%@", [error localizedDescription]) ;
               }
               else
               {
                   NSLog(@"File saved at : %@",localFilePath);
               }
           }
           
           UIViewController *unityController = UnityGetGLViewController();
           
           MPMoviePlayerViewController *playerViewController  = [[MPMoviePlayerViewController alloc]initWithContentURL:[NSURL fileURLWithPath:localFilePath]];
           
           [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(movieFinishedCallback:)
            
                                                      name:MPMoviePlayerPlaybackDidFinishNotification
            
                                                    object:[playerViewController moviePlayer]];
           
           MPMoviePlayerController *player = [playerViewController moviePlayer];
           playerViewController.moviePlayer.fullscreen = YES;
           playerViewController.moviePlayer.scalingMode = MPMovieScalingModeFill;
           
           [unityController.view addSubview:playerViewController.view];
           [unityController presentMoviePlayerViewControllerAnimated:playerViewController];
           
           
              [unityController.view addSubview:playerViewController.view];
           [player play];
           
           UnitySendMessage(CALLBACK_OBJECT, CALLBACK_PICK_METHOD, [localFilePath UTF8String]);
           [self dismissPicker];
       }
         else if( self.mediaMode == 5)
         {
             NSURL *url = [info objectForKey:UIImagePickerControllerMediaURL];
             // Save video to app document directory
             
             NSString *filePath = [url path];
             NSString *pathExtension = [filePath pathExtension] ;
             NSString *localFilePath ;
             if ([pathExtension length] > 0)
             {
                 NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES) ;
                 NSString *documentsDirectory = [paths objectAtIndex:0];
                 
                 localFilePath = [documentsDirectory stringByAppendingPathComponent:[NSString stringWithFormat:@"%@", [filePath lastPathComponent]]];
                 
                 // Method last path component is used here, so that each video saved will get different name.
                 
                 NSError *error = nil ;
                 BOOL res = [[NSFileManager defaultManager] moveItemAtPath:filePath toPath:localFilePath error:&error] ;
                 
                 if (!res)
                 {
                     NSLog(@"%@", [error localizedDescription]) ;
                 }
                 else
                 {
                     NSLog(@"File saved at : %@",localFilePath);
                 }
             }
             
             UnitySendMessage(CALLBACK_OBJECT, CALLBACK_PICK_METHOD, [localFilePath UTF8String]);
             [self dismissPicker];
             
         }
    }
}



- (void) movieFinishedCallback:(NSNotification*) aNotification {
    MPMoviePlayerController *player = [aNotification object];
    [[NSNotificationCenter defaultCenter] removeObserver:self name:MPMoviePlayerPlaybackDidFinishNotification object:player];
    player.fullscreen = NO;
    [player.view removeFromSuperview];
}

//save file into gallery callback
- (void)video:(NSString *)videoPath didFinishSavingWithError:(NSError *)error contextInfo:(void *)contextInfo
{
    if (error) {
        // save error
    }else {
        NSLog(@"%@",videoPath);
       
        // processing
    }
}


- (UIImage *)resizeImage:(UIImage *)sourceImage toMaxWidthAndHeight:(CGFloat)maxValue {
    CGSize imageSize = sourceImage.size;
    
    CGFloat width = imageSize.width;
    CGFloat height = imageSize.height;
    
    if (width > height && width > maxValue) {
        height = height * (maxValue / width);
        width = maxValue;
    }else if (height > width && height > maxValue) {
        width = width * (maxValue / height);
        height = maxValue;
    }else {
        return sourceImage;
    }
    
    UIGraphicsBeginImageContext(CGSizeMake(width, height));
    [sourceImage drawInRect:CGRectMake(0, 0, width, height)];
    
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    return newImage;
}


- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    UnitySendMessage(CALLBACK_OBJECT, CALLBACK_METHOD_FAILURE, MESSAGE_FAILED_CAPTURE);
    
    [self dismissPicker];
}

- (void)dismissPicker
{
    self.outputFileName = nil;
    
    if (self.captureController != nil) {
        [self.captureController dismissViewControllerAnimated:YES completion:^{
            self.captureController = nil;
        }];
    }
    
}


- (UIImage *)fixOrientation:(UIImage *)img {
    // No-op if the orientation is already correct
    if (img.imageOrientation == UIImageOrientationUp) return img;
    
    // We need to calculate the proper transformation to make the image upright.
    // We do it in 2 steps: Rotate if Left/Right/Down, and then flip if Mirrored.
    CGAffineTransform transform = CGAffineTransformIdentity;
    
    switch (img.imageOrientation) {
        case UIImageOrientationDown:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, img.size.width, img.size.height);
            transform = CGAffineTransformRotate(transform, M_PI);
            break;
            
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
            transform = CGAffineTransformTranslate(transform, img.size.width, 0);
            transform = CGAffineTransformRotate(transform, M_PI_2);
            break;
            
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, 0, img.size.height);
            transform = CGAffineTransformRotate(transform, -M_PI_2);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationUpMirrored:
            break;
    }
    
    switch (img.imageOrientation) {
        case UIImageOrientationUpMirrored:
        case UIImageOrientationDownMirrored:
            transform = CGAffineTransformTranslate(transform, img.size.width, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
            
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRightMirrored:
            transform = CGAffineTransformTranslate(transform, img.size.height, 0);
            transform = CGAffineTransformScale(transform, -1, 1);
            break;
        case UIImageOrientationUp:
        case UIImageOrientationDown:
        case UIImageOrientationLeft:
        case UIImageOrientationRight:
            break;
    }
    
    // Now we draw the underlying CGImage into a new context, applying the transform
    // calculated above.
    CGContextRef ctx = CGBitmapContextCreate(NULL, img.size.width, img.size.height,
                                             CGImageGetBitsPerComponent(img.CGImage), 0,
                                             CGImageGetColorSpace(img.CGImage),
                                             CGImageGetBitmapInfo(img.CGImage));
    CGContextConcatCTM(ctx, transform);
    switch (img.imageOrientation) {
        case UIImageOrientationLeft:
        case UIImageOrientationLeftMirrored:
        case UIImageOrientationRight:
        case UIImageOrientationRightMirrored:
            // Grr...
            CGContextDrawImage(ctx, CGRectMake(0,0,img.size.height,img.size.width), img.CGImage);
            break;
            
        default:
            CGContextDrawImage(ctx, CGRectMake(0,0,img.size.width,img.size.height), img.CGImage);
            break;
    }
    
    // And now we just create a new UIImage from the drawing context
    CGImageRef cgimg = CGBitmapContextCreateImage(ctx);
    UIImage *img1 = [UIImage imageWithCGImage:cgimg];
    CGContextRelease(ctx);
    CGImageRelease(cgimg);
    return img1;
    
}


+(NSString*)getCurrentTimes{
    
    NSDateFormatter *formatter = [[NSDateFormatter alloc] init];
    
    [formatter setDateFormat:@"YYYY_MM_dd_HH_mm_ss"];
    
    NSDate *datenow = [NSDate date];
    
    NSString *currentTimeString = [formatter stringFromDate:datenow];
    
    NSLog(@"currentTimeString =  %@",currentTimeString);
    
    return currentTimeString;
    
}



@end



