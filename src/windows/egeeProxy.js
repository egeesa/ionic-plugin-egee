/**
 * @author e-GEE SA
 */

module.exports = {
    getversion: function (success, error, message) {
        try {
            success(Egee.Proxy.EgeeProxy.getversion());
        } catch (e) {
            error("Erreur getversion " + e.message);
        }
    },
    
    testwcf: function (success, error, message) {
        try {
            success(Egee.Proxy.EgeeProxy.testwcf());
        } catch (e) {
            error("Erreur testwcf " + e.message);
        }
    },
    
    helloworld: function (success, error, message) {
        try {
            if (!message || !message.length) {
                error("Error, something was wrong with the input string. =>" + message);
            } else {
                //Créer une instance de la classe CS
                //var classe = new Egee.Egee();
                success(Egee.Proxy.EgeeProxy.helloworld(message) + " echo");
            }
        } catch (e) {
            error("Erreur helloworld " + e.message);
        }
    }
};

require("cordova/exec/proxy").add("EgeePlugin", module.exports);