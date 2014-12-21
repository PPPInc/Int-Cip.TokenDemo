<h4>CIP Token Integration</h4>

##### 1.  Add a reference to <a href="https://ppppublic.blob.core.windows.net/webpos/CIP.token.js">CIP.token.js</a> in the ```<head>``` tag of the html page containing your payment form.
```javascript
<head>
<script src="https://ppppublic.blob.core.windows.net/webpos/CIP.token.js"></script>
</head>
```

##### 2.  Add a payment ```<form>``` to your html page.
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
