/**
 * @author e-GEE SA 
 */

function Egee () {
    this._promise = null;
    this._cancelled = false;
}


var egeeProxy = {

    helloworld: function (success, error, message) {
        alert('test windows');
        //Créer une instance de la classe CS
        var classe = new Egee.Egee();
        alert('test windows 2');
        
        return classe.HelloWorld(message);
    }
};

cordova.commandProxy.add("Egee", egeeProxy);
