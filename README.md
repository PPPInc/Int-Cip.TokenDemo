<h4>CIP Token Integration</h4>

##### 1.  Add a reference to <a href="https://ppppublic.blob.core.windows.net/webpos/CIP.token.js">CIP.token.js</a> in the ```<head>``` tag of the html containing your payment form
```javascript
<head>
    <script src="https://ppppublic.blob.core.windows.net/webpos/CIP.token.js"></script>
</head>
```

##### 2.  Add a payment ```<form>``` to your html page

**Note:** Do not add ```name``` attributes to the *Number* or *Expiration* fields (ie the fields that will be tokenized).  
This will ensure these pieces of sensitive data ***do not*** get posted with the form submit event.  You will use ```data-cip``` ```html5``` data attributes to identify the fields to tokenize.  The ```data-``` attributes are ```data-cip="number", data-cip="exp-month", data-cip-"exp-year"```.
```HTML
<form action="/payment" method="POST" id="payment-form">
    
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
    
    <div>
        <button type="submit">Submit</button>
    </div>
    
    <div id="payment-errors"></div>
    
</form>
```

##### 3.  Intercept the form Submit event, create the CIP Token, then post back to your server in the callback
If you're using jQuery, make sure to add a reference to:
```javascript
<head>
    ...
    <script src="http://code.jquery.com/jquery-2.1.3.min.js"></script>
</head>
```
```Javascript
jQuery(function ($) {

    /* You must set your Merchant Name identifier (Public Key) */
    CIP.token.merchantName = 'Merchant1_23f1984001644e1ba7b4ca9506077e81';
    
    $('#payment-form').submit(function (event) {
    
        var $form = $(this);
       
        /* Create the token and append cipToken as a hidden field on the callback */ 
        CIP.token.create($form, function (status, response) {
        
            if (response.error)
            {
                $form.find('#payment-errors').text(response.error.message);
            
            } else {

                var token = response.Token;

                $form.append($('<input type="hidden" name="cipToken" />').val(token));

                $form.get(0).submit();
            }
        });
        
        // Prevent form submission
        return false;
    });
});
```


#### Server Side Integration

##### 4. .Net Integration
Download <a href="https://github.com/PPPInc/Int-Cip.TokenDemo/blob/master/ExternalLibs/CIP.Token.dll">CIP.Token.dll</a> and add a reference to your .csproj file.

```C#
void YourPaymentHandler()
{
    /* Fetch your form post values */
    var cipToken = this.Request.Form["cipToken"].Value;
    var amount = this.Request.Form["amount"].Value;
    
    /* Set your Private Key */
    CIP.Token.ApiKey = "e5932e4dd41742cd81768c6ace7bedc9";
    
    /* Create a Transaction */
    var transaction = new CIP.Transaction()
    {
        Token = payment.CipToken,
        Amount = payment.Amount,
        TransactionType = payment.TransactionType
    };
    
    /* Process the transaction */
    var result = CIP.Token.RunTransaction(transaction);
    
    /* Save the result to your database and/or render the result values to your receipt view */
}
```

##### 5. Custom Integration (.Net)
The REST Endpoint

######Metadata 
http://cip-payment.azurewebsites.net/json/metadata?op=TransactionRequest

######URI
POST http://cip-payment.azurewebsites.net/token/transaction

######Http Request Headers
**content-type** : application/json<br/>
**x-apikey** : 'e5932e4dd41742cd81768c6ace7bedc9'

######Data (Body)
{ 'Token':'String', 'Amount':0, 'TransactionType':'String' }

#####Server Code C# Example
```C#
void YourPaymentHandler()
{
    /* Fetch your form values */
    var cipToken = this.Request.Form["cipToken"].Value;
    var amount = this.Request.Form["amount"].Value;
    
    /* Create the Transaction object to submit to the Web Service. Note TransactionType must be "sale". */
    var transaction = new { Amount = amount, TransactionType = "sale", Token = cipToken };
    
    /* 
        ToDo: Set the x-apikey in the Request header.  Remember this is your Private Key. 
        i.e. HttpRequest.Headers.Add("X-ApiKey", "e5932e4dd41742cd81768c6ace7bedc9")
    */
    
    /* Invoke the Web Service call */
    var result = YourWebServiceCall();
    
    /* Access the results */
    var refNum = result.RefNum;
}

```


#### Test Credentials / Test Card Data
**MerchantName** : *"Merchant1_23f1984001644e1ba7b4ca9506077e81"*<br/>
**MerchantKey** : *"e5932e4dd41742cd81768c6ace7bedc9"*<br/><br/>

**Card Number** : *4000200011112222*<br/>
**Card Expiration Month** : *05*<br/>
**Card Expiration Year** : *15*
