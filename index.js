var Web3 = require('web3');
web3 = new Web3(new Web3.providers.HttpProvider(process.env.GETH_ADDRESS || "http://geth:8110"));

const express = require('express')
const app = express()

app.listen(3000, function() {
  console.log('CryptoPoker Backend listening on port 3000!')
});
