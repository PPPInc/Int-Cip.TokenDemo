<h4>CIP Token Integration</h4>

##### 1.  Add a reference to <a href="https://ppppublic.blob.core.windows.net/webpos/CIP.token.js">CIP.token.js</a> in the ```<head>``` tag of the html containing your payment form
```javascript
<head>
<script src="https://ppppublic.blob.core.windows.net/webpos/CIP.token.js"></script>
</head>
```

##### 2.  Add a payment ```<form>``` to your html page

**Note:** Do not add ```name``` attributes to the *Number* or *Expiration* fields (ie the fields that will be tokenized).  
This will ensure these pieces of sensitive data do not get posted with the form submit event.  You will use ```data-cip``` ```html5``` data attributes to identify the fields to tokenize.
```HTML
<form action="/payment" method="POST" id="payment-form">

    <span id="payment-errors"></span>
    
    <div>
     <span>Amount</span>
     <input type="text" name="amount">
    </div>
    
    <div>
     <span>Card Number</span>
     <input type="text" data-cip="number">
    </div>
    
    <div>
     <span>Expiration Month</span>
     <input type="text" size="2" data-cip="exp-month">
    </div>
    
    <div>
     <span>Expiration Year</span>
     <input type="text" size="2" data-cip="exp-year">
    </div>
    
</form>
```

##### 3.  Intercept the form Submit event and create the CIP Token
```Javascript
jQuery(function ($) {

    /* You must set your Publishable Key */
    CIP.token.merchantName = 'Merchant1_23f1984001644e1ba7b4ca9506077e81';
    
    $('#payment-form').submit(function (event) {
    
        var $form = $(this);
       
       /* Create the token and append cipToken as a hidden field on the callback */ 
       CIP.token.create($form, function (status, response) {

            var token = response.Token;

            $form.append($('<input type="hidden" name="cipToken" />').val(token));

            $form.get(0).submit();

        });
    });

});
```
