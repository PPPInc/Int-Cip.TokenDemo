#### CIP Token Solution
**What is it?**

CIP Token provides an integrated web based payment solution that ensures sensitive credit card data is not posted to the Merchant's server/domain.

**How does it work?**

CIP tokenizes credit card data in the client browser instance and only posts that token (not credit card data) back to their server form POST handler.  The Merchant then performs a server side tokenized transaction using their CIP (Private) Merchant Key.

#### CIP Token Integration

#####Add a reference to <a href="https://ppppublic.blob.core.windows.net/webpos/CIP.token.js">CIP.token.js</a> in the ```<head>``` tag of the html containing your payment form
```javascript
<head>
    <script src="https://ppppublic.blob.core.windows.net/webpos/CIP.token.js"></script>
</head>
```

#####Add a payment ```<form>``` to your html page

**Note:** Do not add ```name``` attributes to the *Number* or *Expiration* fields (ie the fields that will be tokenized).  
This will ensure these pieces of sensitive data ***do not*** get posted with the form submit event.  You will use ```data-cip``` ```html5``` data attributes to identify the fields to tokenize.  The ```data-``` attributes are ```data-cip="number", data-cip="exp-month", data-cip-"exp-year"```.
```HTML
<form action="/" method="POST" id="payment-form">
    
	<div>
	 <div>Transaction Type</div>
	 <select name="transactionType">
	  <option value="sale">Sale</option>
	  <option value="credit">Credit</option>
	 </select>
	</div>
	
    <div>
     <div>Amount</div>
     <input type="text" name="amount">
    </div>
    
    <div>
     <div>Card Number</div>
     <input type="text" data-cip="number">
    </div>
    
    <div>
     <div>Expiration Month</div>
     <input type="text" size="2" data-cip="exp-month">
    </div>
    
    <div>
     <div>Expiration Year</div>
     <input type="text" size="2" data-cip="exp-year">
    </div>
    
    <div>
        <button type="submit">Submit</button>
    </div>
    
    <div id="payment-errors"></div>
    
</form>
```

#####Intercept the form Submit event, create the CIP Token, then post back to your server in the callback
If you're using jQuery, make sure to add a reference to:
```javascript
<head>
    ...
    <script src="http://code.jquery.com/jquery-2.1.3.min.js"></script>
</head>
```
```Javascript
<script>

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

</script>
```

<br/>
#### Server Side Integration

#####.Net Integration
Download <a href="https://ppppublic.blob.core.windows.net/webpos/CIP.Token.dll">CIP.Token.dll</a> and add a reference to your .csproj file.

```C#
void YourPaymentHandler()
{
	/* Fetch your form post values */
	var cipToken = this.Request.Form["cipToken"].Value;
	var amount = this.Request.Form["amount"].Value;
	var transactionType = this.Request.Form["transactionType"].Value;

	/* Set your Private Key */
	CIP.Token.ApiKey = "e5932e4dd41742cd81768c6ace7bedc9";

	/* Create a Transaction */
	var transaction = new CIP.Transaction()
	{
		Token = cipToken,
		Amount = double.Parse(amount),
		TransactionType = transactionType
	};

	/* Process the transaction */
	var result = CIP.Token.RunTransaction(transaction);

	/* Save the result to your database and/or render the result values to your receipt view */
}
```

#####Custom Integration (.Net) (if not using CIP.Token.dll)
The REST Endpoint

######Metadata 
https://psl.chargeitpro.com/json/metadata?op=TransactionRequest

######URI
POST https://psl.chargeitpro.com/token/transaction

######Http Request Headers
**content-type** : application/json<br/>
**x-apikey** : 'e5932e4dd41742cd81768c6ace7bedc9'

######Data (Body) 
{ 'Amount':0, 'TransactionType':'String', 'Token':'String' }

#####Server Code C# Example
```C#
void YourPaymentHandler()
{
    /* Fetch your form values */
    var cipToken = this.Request.Form["cipToken"].Value;
    var amount = this.Request.Form["amount"].Value;
    var transactionType = this.Request.Form["transactionType"].Value;
    
    /* Create the Transaction object to submit to the Web Service. Note TransactionType must be "sale". */
    var transaction = new { Amount = amount, TransactionType = transactionType, Token = cipToken };
    
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
<br/>
####Http Status Codes
#####200 OK
Successful Http Request.
#####401 Unauthorized
Unauthorized Http Request.  Invalid credentials.
#####404 Not Found
Merchant and/or Token not found.
#####417 Expectation Failed
Credit Card validation failed.

<br/>
####Transaction Result Codes
When you invoke the *../token/transaction* REST Service call you will be returned a JSON Response object.

{<br/>
	"AuthAmount": 0.01,<br/>
	"AuthCode": "332552",<br/>
	"Error": "Approved",<br/>
	"RefNum": "61379914",<br/>
	"**Result**": "Approved",<br/>
	"**ResultCode**": "A",<br/>
	"Status": "Pending",<br/>
	"Date": "/Date(-62135596800000-0000)/"<br/>
}<br/>
#####Result Values
| ResultCode    | Result        | Meaning     |
| ------------- | ------------- | ----------- |
| A  | Approved  | Transaction was Approved |
| D  | Declined  | Transaction was Declined |
| E  | Error | There was an error while processing the transaction |
| V  | Verification | Verification is required |

<br/>
#### Test Credentials / Test Card Data
**MerchantName** : *"Merchant1_23f1984001644e1ba7b4ca9506077e81"*

**MerchantKey** : *"e5932e4dd41742cd81768c6ace7bedc9"*

**Card Number** : *4000200011112222*

**Card Expiration Month** : *05*

**Card Expiration Year** : *15*
