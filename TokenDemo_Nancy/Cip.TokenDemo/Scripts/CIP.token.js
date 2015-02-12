var CIP = CIP || {};

CIP.token = new function () {

    var self = this;
    self.url = 'https://psl.chargeitpro.com/token/card.json';
    //self.url = 'http://localhost:57192/token/card.json';
    self.merchantName = null;
    self.card = {};

    self.create = function (form, callback) {

        var expMonth = null, expYear = null;

        /* Create JSON Object */
        for (i = 0; i < form[0].elements.length; i++) {

            if (form[0].elements[i].getAttribute('data-cip') !== null) {

                switch (form[0].elements[i].getAttribute('data-cip')) {
                    case 'exp-month':
                        expMonth = form[0].elements[i].value;
                        break;
                    case 'exp-year':
                        expYear = form[0].elements[i].value;
                        break;
                    default:
                        self.card[form[0].elements[i].getAttribute('data-cip')] = form[0].elements[i].value;
                }
            }
        }

        if (self.card.expiration == null || self.card.expiration == undefined || self.card.expiration == "" && (expMonth !== null && expYear !== null)) self.card.expiration = expMonth + expYear;

        var request = {
            'merchantName': self.merchantName,
            'card': self.card
        };

        /* Validation */
        var error = validate(request);
        if (error) {
            var result = { 'error': error };
            callback(0, result);
            return;
        }

        /* REST Tokenization */
        var xhr = new XMLHttpRequest();
        xhr.open("POST", self.url);
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var response = eval("(" + xhr.responseText + ")");
                response.error = false;
                callback(xhr.status, response);
            }
        }

        xhr.send(JSON.stringify(request));
    };
}

function validate(request) {

    var error = null;

    if (request === undefined || request === null) error = { 'message': 'Invalid request' };

    if (request.merchantName === undefined || request.merchantName === null) error = { 'message': 'Merchant Name required' };

    if (request.card === undefined || request.card === null
        || request.card.number === undefined || request.card.number === null
        || request.card.expiration === undefined || request.card.expiration == null) error = { 'message': 'Card Number and Expiration Date required' };

    return error;
};