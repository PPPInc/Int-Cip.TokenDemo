/*
 * These functions need to be implemented by consuming Developers.
 */
var OnResultFunction;
var OnEchoFunction;
var OnQuestionFunction;

/*
 * This variable needs to be set by consuming Developers.
 */
var UserName;
var ControllerName;

/*
 * These are functions that can be consumed by Developers and should not be assigned to or overwritten.
 */
var answerYesFunction;
var answerNoFunction;
var echoFunction;
var creditSaleFunction;
var voidFunction;
var requestSignatureFunction;

/*
 * private variables
 */
var _remoteHub;
var _connected = false;

/*
 * private functions
 */
var _connect;

$(function() {

    _connect = function (done, fail) {
        if (UserName === "") {
            alert("UserName not set.");
            return;
        }
        if (ControllerName === "") {
            alert("ControllerName not set.");
            return;
        }

        //var connection = $.hubConnection('https://api-staging.chargeitpro.com');

        var connection = $.hubConnection('http://localhost:811');

        connection.qs = { "userName": UserName };

        _remoteHub = connection.createHubProxy('ChatHub');

        _remoteHub.on('send', function(from, message) {
            var result = JSON.parse(message);
            if (result.Action === 'Echo') {
                if (OnEchoFunction)
                    OnEchoFunction(result.Data);
                return;
            } else if (result.Action === 'Question') {
                if (OnQuestionFunction)
                    OnQuestionFunction(result.Data);
                return;
            } else if (result.Action === 'Result') {
                if (result.ResultFields) {
                    if (OnResultFunction)
                        OnResultFunction(result);
                } else {
                    alert("No Result returned from remote hardware.");
                }
                return;
            }
        });

        connection.start().done(function() {
            _connected = true;
            console.log("Connected");
            done();
        }).fail(function(error) {
            _connected = false;
            console.log(error);
            fail();
        });

        connection.error(function(error) {
            _connected = false;
            console.log('SignalR error: ' + error);
        });
    };

    answerYesFunction = function() {
        var yesMessage = { Action: "Answer", Success: true };
        if (!_connected)
            _connect(function() {
                _remoteHub.invoke('send', ControllerName, JSON.stringify(yesMessage));
            }, function() { alert("Error connecting."); });
        else {
            _remoteHub.invoke('send', ControllerName, JSON.stringify(yesMessage));
        }
    };

    answerNoFunction = function() {
        var noMessage = { Action: "Answer", Success: false };
        if (!_connected)
            _connect(function() {
                _remoteHub.invoke('send', ControllerName, JSON.stringify(noMessage));
            }, function() { alert("Error connecting."); });
        else {
            _remoteHub.invoke('send', ControllerName, JSON.stringify(noMessage));
        }
    };

    creditSaleFunction = function(deviceName, amount, accountNumber, billingName, expDate, cvv, street, zip) {
        var csMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "CreditSale", Amount: amount, AccountNumber: accountNumber, BillingName: billingName, ExpDate: expDate, CVV: cvv, Street: street, Zip: zip, DeviceName: deviceName }) };
        if (!_connected)
            _connect(function() {
                _remoteHub.invoke('send', ControllerName, JSON.stringify(csMessage));
            }, function() { alert("Error connecting."); });
        else {
            _remoteHub.invoke('send', ControllerName, JSON.stringify(csMessage));
        }
    };

    echoFunction = function(message) {
        var eMessage = { Action: "Echo", Data: message };
        if (!_connected)
            _connect(function() {
                _remoteHub.invoke('send', ControllerName, JSON.stringify(eMessage));
            }, function() { alert("Error connecting."); });
        else {
            _remoteHub.invoke('send', ControllerName, JSON.stringify(eMessage));
        }
    };

    voidFunction = function(deviceName, uniqueTransRef) {
        var vMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "Void", UniqueTransRef: uniqueTransRef, DeviceName: deviceName }) };
        if (!_connected)
            _connect(function() {
                _remoteHub.invoke('send', ControllerName, JSON.stringify(vMessage));
            }, function() { alert("Error connecting."); });
        else {
            _remoteHub.invoke('send', ControllerName, JSON.stringify(vMessage));
        }
    };

    requestSignatureFunction = function(deviceName) {
        var rsMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "RequestSignature", DeviceName: deviceName }) };
        if (!_connected)
            _connect(function() {
                _remoteHub.invoke('send', ControllerName, JSON.stringify(rsMessage));
            }, function() { alert("Error connecting."); });
        else {
            _remoteHub.invoke('send', ControllerName, JSON.stringify(rsMessage));
        }
    };
});