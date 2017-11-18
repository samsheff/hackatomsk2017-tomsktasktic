var Web3 = require('web3');
web3 = new Web3(new Web3.providers.HttpProvider(process.env.GETH_ADDRESS || "http://geth:8110"));

const express = require('express')
const app = express()

app.get('/balance', function(req, res) {
  var balance = web3.eth.getBalance(req.query.address).then(function(balance) {
    res.send({balance: balance.toString(10)});
  });
});

app.listen(3000, function() {
  console.log('CryptoPoker Backend listening on port 3000!')
});
