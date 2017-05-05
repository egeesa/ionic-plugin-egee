/**
 * @author e-GEE SA 
 */

function Egee () {
    this._promise = null;
    this._cancelled = false;
}


var egeeProxy = {

    helloworld: function (message, successCallback, errorCallback, options) {
        //Créer une instance de la classe CS
        var classe = new Egee.Egee();
        return classe.HelloWorld(message);
    }
};

cordova.commandProxy.add("Egee", egeeProxy);
