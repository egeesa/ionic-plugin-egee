package io.ionic.egee;

import org.apache.cordova.CallbackContext;
import org.apache.cordova.CordovaInterface;
import org.apache.cordova.CordovaPlugin;
import org.apache.cordova.CordovaWebView;
import org.apache.cordova.PluginResult;
import org.apache.cordova.PluginResult.Status;
import org.json.JSONArray;
import org.json.JSONException;

import android.content.Context;
import android.graphics.Rect;
import android.util.DisplayMetrics;
import android.view.View;
import android.view.ViewTreeObserver.OnGlobalLayoutListener;
import android.view.inputmethod.InputMethodManager;

// import additionally required classes for calculating screen height
import android.view.Display;
import android.graphics.Point;
import android.os.Build;

public class IonicEgee extends CordovaPlugin {

    public void initialize(CordovaInterface cordova, CordovaWebView webView) {
        super.initialize(cordova, webView);
    }

    public boolean execute(String action, JSONArray args, final CallbackContext callbackContext) throws JSONException {
        if ("totodelasvegas".equals(action)) {
            // cordova.getThreadPool().execute(new Runnable() {
            //     public void run() {
            //         callbackContext.success("totodelasvegas"); // Thread-safe.
            //     }
            // });
            String message = args.getString(0);

            callbackContext.success("Salut " + message); // Thread-safe.
            return true;
        }
        // if ("init".equals(action)) {
        //     cordova.getThreadPool().execute(new Runnable() {
        //         public void run() {
        //             PluginResult dataResult = new PluginResult(PluginResult.Status.OK);
        //             dataResult.setKeepCallback(true);
        //             callbackContext.sendPluginResult(dataResult);
        //         }
        //     });
        //     return true;
        // }
        return false;  // Returning false results in a "MethodNotFound" error.
    }


}


