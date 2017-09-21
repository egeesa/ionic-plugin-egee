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
import android.util.Log;

import com.diehl.metering.izar.license.lic.License;
import com.diehl.metering.izar.module.readout.text.impl.RadioInterpret;
import com.diehl.metering.izar.module.readout.text.impl.Receiver;
import com.diehl.metering.izar.module.readout.api.v1r0.IReadoutInterpretSPI;
import java.lang.Exception;

public class EgeePlugin extends CordovaPlugin {

    public void initialize(CordovaInterface cordova, CordovaWebView webView) {
        super.initialize(cordova, webView);
    }

    public boolean execute(String action, JSONArray args, final CallbackContext callbackContext) throws JSONException {
        if ("helloworld".equals(action)) {
            // cordova.getThreadPool().execute(new Runnable() {
            //     public void run() {
            //         callbackContext.success("totodelasvegas"); // Thread-safe.
            //     }
            // });
            String message = args.getString(0);

            callbackContext.success("Salut " + message); // Thread-safe.
            return true;
        }
        if ("sappelLicense".equals(action)) {
            // cordova.getThreadPool().execute(new Runnable() {
            //     public void run() {
            //         callbackContext.success("totodelasvegas"); // Thread-safe.
            //     }
            // });
            try {
                verifierLicence();
                String message = args.getString(0);

                callbackContext.success("License ok " + message); // Thread-safe.
            } catch (Exception e) {
                // No license
                callbackContext.error("Erreur lors de la lecture du fichier de license" + e);
            }

            return true;
        }
        if ("sappelTraitement".equals(action)) {
            try {
                String frame = args.getString(0);
                verifierLicence();
                String result = RadioInterpret.INSTANCE.interpret(frame);
                callbackContext.success(result); // Thread-safe.
            } catch (Exception e) {
                callbackContext.error("Erreur lors de l'interprétation de la trame" + e);
            }
            return true;
        }
        if ("sappelBluetooth".equals(action)) {
            cordova.getActivity().runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    final String adresseMAC = args.getString(0);
                    final String messageSupplementaire = "";
                    Log.d("RECEIVER", "sappelBluetooth");
                    verifierLicence();

                    Receiver.start("00:12:F3:18:E0:3E", new IReceiverCallback() {

                        @Override
                        public void onKeepAlive() {
                            // TODO Auto-generated method stub
                            Log.d("RECEIVER", "onKeepAlive");
                        }

                        @Override
                        public void onFrame(final String arg0, final String arg1) {
                            System.out.println(arg0 + " " + arg1);
                            Log.d("RECEIVER", "Trame reçue" + s + " " + s1);
                        }

                        @Override
                        public void onError(final Exception arg0) {
                            // TODO Auto-generated method stub
                            Log.e("RECEIVER", "Une erreur est survenue", e);
                        }

                        @Override
                        public void onConnectionClosed() {
                            // TODO Auto-generated method stub
                            Log.d("RECEIVER", "Deconnexion");
                        }
                    });

                }
            });

            try {
                Thread.sleep(100000);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
            return true;
        }
        return false; // Returning false results in a "MethodNotFound" error.
    }

    private void verifierLicence() {
        License.getInstance().read(
                "010000000700E651400000000F45474545000000000000000000000000000000001F371428D0F423EDACCE8748E9713D952E2D576E8C9455B1F9B8EF4F7215664F3DD28E910F56B7E91EDA85FFC25082E4556CB69ADB4E260C770A1F1F1B7F4482CB90161606D0C40F221A3E51245E9BD95C5B1E4635436537CF9ABBDE51B24279D522022AC5076F1B6AC4459966787B4251AD511FDEE78002A3B47330549B2F39B05B4AFFA4AE28E90818D03D3A940E8F29264240C7401F8B5101729C3D7B34536EFC570EDCDC060AAC9436EDC376396B36972E9D7A03C80D59529F954347769C48D2F18875E9FC42BD440498AD0B5839CDF38797227BB8B0D8A00F8349AC7EA7690827FA87A26D473538D7709FC1DD0F8C3C2360930B664198131ADF682A613B");
        License.getInstance().validate();

        // try {
        //     License.getInstance()
        //             .readHexStream(IReadoutInterpretSPI.class.getResourceAsStream("resources/EGEE.lic"));
        // } catch (Exception e) {
        //     Log.e("RECEIVER", "verifierLicence : No license", e);
        // }
    }

}
