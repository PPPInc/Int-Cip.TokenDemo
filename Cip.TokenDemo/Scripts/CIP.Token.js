var CIP = CIP || {};

CIP.Token = new function () {

    var self = this;
    self.merchantName = null;
    self.card = {};

    self.create = function (form, callback) {

        var expMonth = null, expYear = null;

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
                        self.card[form[0].elements[i].getAttribute('data-cip')] = form[0].elements[i].getAttribute('value');
                }
            }
        }

        self.card.expiration = expMonth + expYear;

        var request = {
            'MerchantName': self.merchantName,
            'Card': self.card
        };

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "http://cip-payment.azurewebsites.net/token/card.json");
        xhr.setRequestHeader('Content-Type', 'application/json');
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                callback(xhr.status, eval("(" + xhr.responseText + ")"));
            }
        }

        xhr.send(JSON.stringify(request));
    };
}