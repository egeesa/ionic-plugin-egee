/**
 * @author e-GEE SA
 */

module.exports = {
    helloworld: function (success, error, message) {
        try {
            if (!message || !message.length) {
                error("Error, something was wrong with the input string. =>" + message);
            }
            else {
                //Créer une instance de la classe CS
                //var classe = new Egee.Egee();
                success(Egee.Proxy.EgeeProxy.helloWorld(message) + " echo");
            }
        } catch (e) {
            error("Erreur helloworld " + e.message);
        }
    }
};

require("cordova/exec/proxy").add("EgeePlugin", module.exports);