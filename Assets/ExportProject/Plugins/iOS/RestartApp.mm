#import <UIKit/UIKit.h>

extern "C" void _restartApp() {
    // 获取当前的UIApplication对象
    UIApplication *application = [UIApplication sharedApplication];
    // 获取当前的rootViewController
    UIViewController *rootViewController = application.keyWindow.rootViewController;
    
    // 创建一个新线程来重启应用
    dispatch_async(dispatch_get_main_queue(), ^{
        [application performSelector:@selector(terminateWithSuccess)];
        [rootViewController dismissViewControllerAnimated:NO completion:^{
            // 重新启动应用
            [application performSelector:@selector(presentViewController:animated:completion:) withObject:rootViewController afterDelay:0.1f];
        }];
    });
}
