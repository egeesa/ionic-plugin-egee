package io.ionic.egee;

import org.apache.cordova.CallbackContext;
import org.apache.cordova.CordovaInterface;
import org.apache.cordova.CordovaPlugin;
import org.apache.cordova.CordovaWebView;
import org.apache.cordova.PluginResult;
import org.apache.cordova.PluginResult.Status;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.xml.sax.SAXException;

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
import android.util.Log;

import com.diehl.metering.izar.license.lic.License;
import com.diehl.metering.izar.module.readout.text.impl.RadioInterpret;
import com.diehl.metering.izar.module.readout.text.impl.Receiver;
import com.diehl.metering.izar.module.readout.text.impl.Receiver.IReceiverCallback;
import com.sun.xml.internal.bind.v2.runtime.unmarshaller.UnmarshallingContext.State;
import com.diehl.metering.izar.module.readout.api.v1r0.IReadoutInterpretSPI;
import java.lang.Exception;

public class EgeePlugin extends CordovaPlugin {

    protected static final String TAG = "EgeePlugin";
    protected CallbackContext context;

    public void initialize(CordovaInterface cordova, CordovaWebView webView) {
        super.initialize(cordova, webView);
    }

    @Override
    public boolean execute(String action, JSONArray data, CallbackContext callbackContext) throws JSONException {
        context = callbackContext;
        boolean result = true;
        try {
            if (action.equals("hello")) {

                String input = data.getString(0);
                String output = "EGEE Sappel Android: " + input;
                callbackContext.success(output);

            } else if (action.equals("getLicense")) {
                try {
                    String myLicense = data.getString(0);
                    if (myLicense == "") {
                        myLicense = "010000001000E71F200000001F454745450000000000000000000000000000000022C184F465E9D474F85F152C8899D9E4D3DE103DF2B7F6F58387978F426CF4219555EF54322B56093F38F87AB4FE95B442C8BADCD350C2D13BF7CC04374555CEF5CDA422E67E1E3CE4C9B3B2FA597962F0E0225A5B3634A72D1B2F60773A8D2CECB6117200601989C1AF84E08865A0EE693FB5D33E9441A3B7918C3621C17F8879201512C0D39F1C7059F99BE1AFB83A50CBD3B2CD6CEB4000DB2375EF943F3EB6536BA090B8164DC17FB5632E51E8FD22208C7D2F34F067EB5E8B654147E7086C5873DCECF9388EEB18B08FEB63D8D683C299410AD6714A144A90A852ED267779DE669F26E02D2B2807328EF9EB0F0653BF855A6814DB99A34F04A06B9459A5";
                    }
                    ;
                    License.getInstance().read(myLicense);
                    License.getInstance().validate();
                    String output = License.getInstance().validate();
                    context.success(output);
                    context.error("Licence invalide");
                } catch (Exception e) {
                    handleException(e);
                }

            } else if (action.equals("getTelegram")) {
                try {

                    String frame = ReceiverStatic.pollFrames();

                    callbackContext.success(frame);
                    callbackContext.error("Echec getTelegram");
                } catch (Exception e) {
                    handleException(e);
                }

            } else if (action.equals("startBluetoothTelegrams")) {
                try {
                    String output = "";
                    String adressMac = data.getString(0);
                    if (adressMac != "") {
                        // Connection between device and Receive
                        output = ReceiverStatic.start(adressMac);
                    }
                    callbackContext.success("Start Bleutooth connection:  " + output);
                } catch (Exception e) {
                    handleException(e);
                }
            } else if (action.equals("stopBluetoothTelegrams")) {
                try {
                    // Stop connection between device and Receive
                    ReceiverStatic.stop();
                    String output = args.getString(0);
                    callbackContext.success("Stop Bleutooth connection:  " + output);
                } catch (Exception e) {
                    handleException(e);
                }
            } else if (action.equals("interpreter")) {
                try {
                    // Radio frame interpreter
                    String frame = data.getString(0);
                    if (frame != "") {
                        String output = RadioInterpret.INSTANCE.interpret(frame);
                    }
                    callbackContext.success(output);
                    callbackContext.error("Echec interpretation du frame");
                } catch (Exception e) {
                    handleException(e);
                }
            } else if (action.equals("headInterpreter")) {
                try {
                    // Radio frame head interpreter
                    String frame = data.getString(0);
                    if (frame != "") {
                        String output = RadioInterpret.INSTANCE.interpretHead(frame);
                    }
                    callbackContext.success(output);
                    callbackContext.error("Echec interpretation de l'entête du frame");
                } catch (Exception e) {
                    handleException(e);
                }
            } else {
                handleError("Invalid action");
                result = false;
            }
        } catch (Exception e) {
            handleException(e);
            result = false;
        }
        return result;
    }

    private void handleError(String errorMsg) {
        try {
            Log.e(TAG, errorMsg);
            context.error(errorMsg);
        } catch (Exception e) {
            Log.e(TAG, e.toString());
        }
    }

    private void handleException(Exception exception) {
        handleError(exception.toString());
    }

}
