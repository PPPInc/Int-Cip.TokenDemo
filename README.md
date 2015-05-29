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
	
	<!-- Your Form Data Goes Here -->
	
    </div>
    
    <div>
     <div>Card Number</div>
     <input type="text" data-cip="accountdata">
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
     <div>Billing Name</div>
     <input type="text" data-cip="billingname">
    </div>

    <!-- Add the following 3 fields only for Keyed Transactions (i.e. non-swipe) -->
        
    <div>
     <div>CVV Number</div>
     <input type="text" size="3" data-cip="cvvnumber">
    </div>
    
    <div>
     <div>Billing Street Address</div>
     <input type="text" data-cip="billingstreetaddress">
    </div>
    
    <div>
     <div>Billing Zip</div>
     <input type="text" data-cip="billingzip">
    </div>
    
    <div>
        <button type="submit">Submit</button>
    </div>
    
    <div id="payment-errors"></div>
    
</form>
```

**Note:** For Card Present (i.e. swipe data), there is no need to include the BillingStreetAddress, BillingZip and CVVNumber fields in your form; these values will be automatically be parsed from the swipe.  For Keyed Transactions, **do** include the BillingStreetAddress, BillingZip and CVVNumber fields in your form.  Submitting BillingStreetAddress, BillingZip and CVVNumber will ensure you get the lowest rate for manual transactions. 

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
	var amount = GetYourAmount(); // i.e. 9.99
	var transactionType = GetYourTransactionType(); // i.e. CreditSale
	
	/* This toggles the environment, default points to Sandbox, set to True when migrating to Production */
	CIP.Token.IsSandbox = false;

	/* Set your Private Key */
	CIP.Token.ApiKey = "e5932e4dd41742cd81768c6ace7bedc9";

	/* Create a Transaction */
	var transaction = new CIP.Transaction()
	{
		Token = cipToken,
		Amount = double.Parse(amount),
		TransactionType = transactionType,
		Invoice = "Invoice Name",
		PONumber = "12345",
		OrderId = "98765",
		Description = "Your description",
		BillingAddress = new BillingAddress()
		{ 
			CustomerId = "Your Customer Id",
			FirstName = "John",
			LastName = "Smith",
			Company = "The Billing Company",
			Street = "1 Mockingbird Ln.",
			Street2 = "Apt. 1",
			City = "Eagle",
			State = "ID",
			Zip = "55555-4444",
			Country = "USA",
			Phone = "555-555-5555",
			Email = "email@domain.com"
		},
		ShippingAddress = new ShippingAddress()
		{ 
			FirstName = "Sam",
			LastName = "Smith",
			Company = "The Shipping Company",
			Street = "2 Shady Ln.",
			Street2 = "Apt. 2",
			City = "Boise",
			State = "ID",
			Zip = "55555-4444",
			Country = "USA",
			Phone = "555-555-5555",
			Email = "email1@domain.com"
		},
		Comments = "Your comments"
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
    
    /* This toggles the environment, default points to Sandbox, set to True when migrating to Production */
    var isSandbox = true;
    
    /* Create the Transaction object to submit to the Web Service. Note TransactionType must be "sale". */
    var transaction = new { 
    	Amount = amount, TransactionType = transactionType, 
    	Token = cipToken, Invoice = "Invoice Name", IsSandbox = isSandbox 
    };
    
    /* 
        ToDo: Set the x-apikey in the Request header.  Remember this is your Private Key. 
        i.e. HttpRequest.Headers.Add("X-ApiKey", "e5932e4dd41742cd81768c6ace7bedc9")
    */
    
    /* Invoke the Web Service call */
    var result = YourWebServiceCall();
    
    /* Access the results */
    var refNum = result.UniqueTransId;
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
#####500 Internal Server Error *Note: this will be broken out.
Credit Card validation failed.

<br/>
####Transaction Result Codes
When you invoke the *../token/transaction* REST Service call you will be returned a JSON Response object.
```
{
	"Success": true,
	"Status": "OK",
	"StatusCode": 200,
	"Result":
	{
	   "UniqueTransID": "61614252",
	   "BatchNumber": "159086",
	   "ResultMessage": "014860",
	   "ResultStatus": true,
	   "ApprovalNumberResult": "014860",
	   "AmountBalance": "0.00",
	   "AmountProcessed": "0.01",
	   "AVSResponseCode": "YYY",
	   "AVSResponseText": "Address: Match & 5 Digit Zip: Match",
	   "CVVResponseCode": "M",
	   "CVVResponseText": "Match",
	   "AccountCardType": "VS",
	   "AccountExpiryDate": "0515",
	   "TransactionType": "sale",
	   "BillingName": "John Q. Public",
	   "MaskedAccount": "************2222",
	   "AccountEntryMethod": "Keyed",
	   "CreatedOn": "/Date(1423700761447)/"
	}
}
```

<br/>
####Errors
Error messages will be returned in the Error Field.

```
{
	"Success": false,
	"Status": "InternalServerError",
	"StatusCode": 500,
	"Error":
	{
	   "Message": "Credit card has expired. [GW:17]"
	}
}
```

#### Test Credentials / Test Card Data
**MerchantName** : *"Merchant1_23f1984001644e1ba7b4ca9506077e81"*

**MerchantKey** : *"e5932e4dd41742cd81768c6ace7bedc9"*

**Card Number** : *4000200011112222*

**Card Expiration Month** : *05*

**Card Expiration Year** : *15*
