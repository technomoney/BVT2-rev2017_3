// Tangled Reality Studios modified - 8/30/18 - the import of UIKit and Foundation were not in the original
#import <UIKit/UIKit.h>
#import <Foundation/Foundation.h>
#if defined(UNITY_4_0) || defined(UNITY_5_0)
#import "iPhone_View.h"
#else
extern UIViewController* UnityGetGLViewController();
#endif

// Tangled Reality Studios modified - 8/30/18 - the PlatformDependentMedia, OptionalUrl, and OptionalText objects were all not in the original and added by me
@interface PlatformDependentMedia:UIActivityItemProvider <UIActivityItemSource>
@property (nonatomic, copy) NSString *filePath;
- (PlatformDependentMedia *)initWithFilePath:(NSString *)filePath;
@end
@implementation PlatformDependentMedia
@synthesize filePath;

- (PlatformDependentMedia *)initWithFilePath:(NSString *)filePath {
    if (self = [super initWithPlaceholderItem:filePath]) {
        self.filePath = filePath;
    }
    return self;
}

- (id)activityViewControllerPlaceholderItem:(UIActivityViewController *)activityViewController {
    return [[UIImage alloc] init];
}

- (id)activityViewController:(UIActivityViewController *)activityViewController itemForActivityType:(UIActivityType)activityType {
    if ([activityType isEqualToString:@"com.facebook.Messenger.ShareExtension"]) {
        return filePath.length != 0 ? [[UIImage alloc] initWithContentsOfFile:filePath] : nil;
    }
    
    return filePath.length != 0 ? [NSURL fileURLWithPath:filePath] : nil;
}
@end

@interface OptionalPlatformDependentMedia:UIActivityItemProvider <UIActivityItemSource>
@property (nonatomic, copy) NSString *filePath;
- (OptionalPlatformDependentMedia *)initWithFilePath:(NSString *)filePath;
@end
@implementation OptionalPlatformDependentMedia
@synthesize filePath;

- (OptionalPlatformDependentMedia *)initWithFilePath:(NSString *)filePath {
    if (self = [super initWithPlaceholderItem:filePath]) {
        self.filePath = filePath;
    }
    return self;
}

- (id)activityViewControllerPlaceholderItem:(UIActivityViewController *)activityViewController {
    return [[UIImage alloc] init];
}

- (id)activityViewController:(UIActivityViewController *)activityViewController itemForActivityType:(UIActivityType)activityType {
    if ([activityType isEqualToString:UIActivityTypePostToFacebook]) {
        return nil;
    } else if ([activityType isEqualToString:UIActivityTypePostToTwitter]) {
        return nil;
    } else if ([activityType isEqualToString:@"com.facebook.Messenger.ShareExtension"]) {
        return filePath.length != 0 ? [[UIImage alloc] initWithContentsOfFile:filePath] : nil;
    }
    
    return filePath.length != 0 ? [NSURL fileURLWithPath:filePath] : nil;
}
@end

@interface OptionalUrl:UIActivityItemProvider <UIActivityItemSource>
@property (nonatomic, copy) NSString *urlPath;
- (OptionalUrl *)initWithUrlPath:(NSString *)urlPath;
@end
@implementation OptionalUrl
@synthesize urlPath;

- (OptionalUrl *)initWithUrlPath:(NSString *)urlPath {
    if (self = [super initWithPlaceholderItem:urlPath]) {
        self.urlPath = urlPath;
    }
    return self;
}

- (id)activityViewControllerPlaceholderItem:(UIActivityViewController *)activityViewController {
    return urlPath.length != 0 ? [[NSURL alloc] initWithString:urlPath] : [[NSURL alloc] initWithString:@""];
}

- (id)activityViewController:(UIActivityViewController *)activityViewController itemForActivityType:(UIActivityType)activityType {
    if ([activityType isEqualToString:UIActivityTypePostToFacebook]) {
        return nil;
    } else if ([activityType isEqualToString:UIActivityTypePostToTwitter]) {
        return nil;
    } else if ([activityType isEqualToString:@"com.facebook.Messenger.ShareExtension"]) {
        return nil;
    }
    
    return urlPath.length != 0 ? [[NSURL alloc] initWithString:urlPath] : nil;
}
@end

@interface OptionalText:UIActivityItemProvider <UIActivityItemSource>
@property (nonatomic, copy) NSString *text;
- (OptionalText *)initWithText:(NSString *)text;
@end
@implementation OptionalText
@synthesize text;

- (OptionalText *)initWithText:(NSString *)text {
    if (self = [super initWithPlaceholderItem:text]) {
        self.text = text;
    }
    return self;
}

- (id)activityViewControllerPlaceholderItem:(UIActivityViewController *)activityViewController {
    return text;
}

- (id)activityViewController:(UIActivityViewController *)activityViewController itemForActivityType:(UIActivityType)activityType {
    /*
     if ([activityType isEqualToString:UIActivityTypePostToFacebook]) {
     return nil;
     }
     */
    return text;
}
@end

// Tangled Reality Studios modified - 8/30/18 - the url parameter and its use, the prioritizeFile paremeter, and the use of the classes added above are not in the original

// Credit: https://github.com/ChrisMaire/unity-native-sharing

extern "C" void _NativeShare_Share( const char* files[], int filesCount, char* subject, const char* text, const char* url, bool prioritizeFile)
{
    NSMutableArray *items = [NSMutableArray new];
    if(filesCount == 0 || !prioritizeFile)
        [items addObject:[[NSURL alloc] initWithString: [NSString stringWithUTF8String:url]]];
    else if( strlen( url ) > 0 )
        [items addObject:[[OptionalUrl alloc] initWithUrlPath: [NSString stringWithUTF8String:url]]];
    
    if( strlen( text ) > 0 )
        [items addObject:[[OptionalText alloc] initWithText: [NSString stringWithUTF8String:text]]];
    
    // Credit: https://answers.unity.com/answers/862224/view.html
    for( int i = 0; i < filesCount; i++ )
    {
        NSString *filePath = [NSString stringWithUTF8String:files[i]];
        if(prioritizeFile)
            [items addObject:[[PlatformDependentMedia alloc] initWithFilePath:filePath]];
        else
            [items addObject:[[OptionalPlatformDependentMedia alloc] initWithFilePath:filePath]];
    }
    
    UIActivityViewController *activity = [[UIActivityViewController alloc] initWithActivityItems:items applicationActivities:nil];
    if( strlen( subject ) > 0 )
        [activity setValue:[NSString stringWithUTF8String:subject] forKey:@"subject"];
    
    UIViewController *rootViewController = UnityGetGLViewController();
    if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone ) // iPhone
        [rootViewController presentViewController:activity animated:YES completion:nil];
    else // iPad
    {
        // Tangled Reality Studios modified - 8/30/18 - updated presentation style to avoid depracated functions
        activity.modalPresentationStyle = UIModalPresentationPopover;
        activity.popoverPresentationController.sourceView = rootViewController.view;
        [rootViewController presentViewController:activity animated:YES completion:nil];
    }
}
