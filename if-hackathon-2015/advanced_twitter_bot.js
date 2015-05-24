var request = require("request");
var Codebird = require("codebird");

var cb = new Codebird();
cb.setConsumerKey("CONSUMER_KEY", "CONSUMER_SECRET");
cb.setToken("TOKEN_KEY", "TOKEN_SECRET");

function tweet(message) {
    var params = {
        status: message
    };
    cb.__call(
        "statuses_update",
        params,
        function (reply) {
            console.log(reply);
        }
    );
}