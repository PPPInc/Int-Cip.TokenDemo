/*
 * These functions need to be implemented by consuming Developers.
 */
var OnResultFunction; //Function to be called when transaction results are returned from remote hardware.
var OnEchoFunction; //Function to be called when an echo response is returned from remote hardware.
var OnQuestionFunction; //Function to be called when remote hardware requires input from the POS to finish a transaction.

/*
 * These variables need to be set by consuming Developers.
 */
var UserName;
var ControllerName;

/*
 * These are functions that can be consumed by Developers and should not be assigned to or overwritten.
 */
var answerYesFunction; //Call this function to respond with "YES" to a question request from remote hardware.
var answerNoFunction; //Call this function to respond with "NO" to a question request from rmote hardware.
var echoFunction; //Call this function to test connectivity between your web-based POS and the remote hardware controller.
var debitSaleFunction; //Call this function to process a debit sale.
var creditSaleFunction; //Call this function to process a credit sale.
var creditReturnFunction; //Call this function to process a credit return.
var creditAuthFunction; //Call this function to authorize a transaction.
var creditForceFunction; //Call this function to force a credit sale.
var creditAddTipFunction; //Call this function to add a tip to a previous transaction.
var voidFunction; //Call this function to void a transaction.
var requestSignatureFunction; //Call this function to request a signature.
var saveCreditCardFunction; //Call this function to tokenize a credit card.
var displayTextFunction; //Call this function to display a message on the device screen.


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

    var doTransaction = function(message) {
        if (!_connected)
            _connect(function () {
                _remoteHub.invoke('send', ControllerName, JSON.stringify(message));
            }, function () { alert("Error connecting."); });
        else {
            _remoteHub.invoke('send', ControllerName, JSON.stringify(message));
        }
    };

    answerYesFunction = function() {
        var yesMessage = { Action: "Answer", Success: true };
        doTransaction(yesMessage);
    };

    answerNoFunction = function() {
        var noMessage = { Action: "Answer", Success: false };
        doTransaction(noMessage);
    };

    creditSaleFunction = function(deviceName, amount, accountNumber, billingName, expDate, cvv, street, zip) {
        var csMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "CreditSale", Amount: amount, AccountNumber: accountNumber, BillingName: billingName, ExpDate: expDate, CVV: cvv, Street: street, Zip: zip, DeviceName: deviceName }) };
        doTransaction(csMessage);
    };

    creditReturnFunction = function(deviceName, amount) {
        var crMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "CreditReturn", Amount: amount, DeviceName: deviceName }) };
        doTransaction(crMessage);
    };

    creditAuthFunction = function(deviceName, amount, accountNumber, billingName, expDate, cvv, street, zip) {
        var caMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "CreditAuth", Amount: amount, AccountNumber: accountNumber, BillingName: billingName, ExpDate: expDate, CVV: cvv, Street: street, Zip: zip, DeviceName: deviceName }) };
        doTransaction(caMessage);
    };

    creditForceFunction = function(deviceName, amount, authCode, uniqueTransRef) {
        var cfMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "CreditForce", Amount: amount, DeviceName: deviceName, VoiceAuthCode: authCode, UniqueTransRef: uniqueTransRef }) };
        doTransaction(cfMessage);
    };

    creditAddTipFunction = function(deviceName, amount, uniqueTransRef) {
        var catMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "CreditAddTip", Amount: amount, DeviceName: deviceName, UniqueTransRef: uniqueTransRef }) };
        doTransaction(catMessage);
    };

    saveCreditCardFunction = function(deviceName) {
        var sccMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "CreditSaveCard", Amount: "0.05", DeviceName: deviceName }) };
        doTransaction(sccMessage);
    };

    debitSaleFunction = function(deviceName, amount) {
        var dsMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "DebitSale", Amount: amount, DeviceName: deviceName }) };
        doTransaction(dsMessage);
    };

    echoFunction = function(message) {
        var eMessage = { Action: "Echo", Data: message };
        doTransaction(eMessage);
    };

    voidFunction = function(deviceName, uniqueTransRef) {
        var vMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "Void", UniqueTransRef: uniqueTransRef, DeviceName: deviceName }) };
        doTransaction(vMessage);
    };

    requestSignatureFunction = function(deviceName) {
        var rsMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "RequestSignature", DeviceName: deviceName }) };
        doTransaction(rsMessage);
    };

    displayTextFunction = function(deviceName, displayText) {
        var dtMessage = { Action: "Transaction", Data: JSON.stringify({ TransactionType: "DisplayText", DeviceName: deviceName, DisplayText: displayText }) };
        doTransaction(dtMessage);
    };
});