<html>
 <head>
  <title>CIP Token PHP Example</title>
	<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap.min.css">
	<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap-theme.min.css">
	<link rel="stylesheet" href="http://cip-token-demo.azurewebsites.net/content/site.css">
	<script src="https://ppppublic.blob.core.windows.net/webpos/CIP.token.js"></script>
	<script src="http://code.jquery.com/jquery-2.1.3.min.js"></script>
 </head>
 <body>
	    
    <h4>Secure Payment Form</h4>

    <form role="form" action="payment.php" method="POST" id="payment-form">

        <div class="row">

            <div class="col-md-6">

                <div class="panel panel-default">

                    <div class="panel-heading"><h3 class="panel-title">Card Information</h3></div>

                    <div class="panel-body">

                        <div class="form-group">
                            <label>Transaction Type</label>
                            <select class="form-control" style="width: 130px;" name="transactionType">
                                <option value="sale">Credit Sale</option>
                                <option value="debit">Debit Sale</option>
                            </select>
                        </div>

                        <div class="form-group">
                            <label>Amount</label>
                            <input type="text" class="form-control input-medium" value="1.03" name="amount">
                        </div>

                        <div class="form-group">
                            <label>Card Number</label>
                            <input type="text" class="form-control input-medium" value="4000200011112222" data-cip="number">
                        </div>

                        <div class="form-group">
                            <label></label>
                            <select class="form-control expire" style="margin: 0 10px 0 0;" data-cip="exp-month">
                                <option value="01">January</option>
                                <option value="02">February</option>
                                <option value="03">March</option>
                            </select>
                            <select class="form-control expire" data-cip="exp-year">
                                <option value="15">2015</option>
                                <option value="16">2016</option>
                            </select>
                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <button class="btn btn-default" type="submit">Submit</button>
                </div>

            </div>

        </div>

        <?php

	   	$ciptoken = (empty($_POST['cipToken'])) ? '' : $_POST['cipToken'];
	   	$amount = (empty($_POST['amount'])) ? '0' : $_POST['amount'];
                
	   	if($ciptoken != null) {

	   		$response = httpRequest($ciptoken, $amount);
	   		echo var_dump($response);
	   	} 

	   	function httpRequest($ciptoken, $amount) {

			$data = array("token" => $ciptoken, "amount" => $amount, "transactionType" => "sale");                                                                    
			$data_string = json_encode($data);                                                   
			 
			$ch = curl_init('https://psl.chargeitpro.com/token/transaction.json');   
			curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, FALSE);
			curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");                                                                     
			curl_setopt($ch, CURLOPT_POSTFIELDS, $data_string);                                                                  
			curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);                                                                      
			curl_setopt($ch, CURLOPT_HTTPHEADER, array(	
				'X-ApiKey: e5932e4dd41742cd81768c6ace7bedc9',                                                                           
				'Content-Type: application/json', 
			    	'Content-Length: ' . strlen($data_string))                        
			);         
			
			$result = curl_exec($ch); 
			
			$http_status = curl_getinfo($ch, CURLINFO_HTTP_CODE);
            
            		echo '$http_status: ' . $http_status . ' ';

			$jsonResult = json_decode($result);

			return $jsonResult;
		}

	?>

    </form>

    <script>

        jQuery(function ($) {

            /* This is the Publishable Key */
            CIP.token.merchantName = 'Merchant1_23f1984001644e1ba7b4ca9506077e81';

            $('#payment-form').find('button').prop('disabled', false);

            $('#payment-form').submit(function (event) {

                var $form = $(this);

                // Disable the submit button and show ajax loader
                $form.find('button').prop('disabled', true);

                CIP.token.create($form, function (status, response) {

                    var token = response.Token;
                    
                    $form.append($('<input type="hidden" name="cipToken" />').val(token));

                    $form.get(0).submit();

                });

                // Prevent the form from submitting with the default action
                return false;
            });
        });

    </script>

 </body>

</html>
