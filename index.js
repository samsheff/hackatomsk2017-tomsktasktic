var Web3 = require('web3');
web3 = new Web3(new Web3.providers.HttpProvider(process.env.GETH_ADDRESS || "http://geth:8110"));

var clickhouse = require('@apla/clickhouse');
ch = new clickhouse(process.env.CLICKHOUSE_ADDRESS || "clickhouse");

const express = require('express')
const app = express()

const CREATE_DATABASE_STRING = "CREATE DATABASE IF NOT EXISTS cryptopoker;"
const CREATE_TABLE_STRING = "CREATE TABLE IF NOT EXISTS cryptopoker.accounts (address String, encryptedWallet String, password String, createdAt DateTime DEFAULT now()) Engine = Log;";

app.get('/balance', function(req, res) {
  var balance = web3.eth.getBalance(req.query.address).then(function(balance) {
    res.send({balance: balance.toString(10)});
  });
});

app.get('/register', function(req, res) {
  var account = web3.eth.accounts.create();
  var encryptedJSON = account.encrypt(account.privateKey, req.query.password);

  console.log("INSERT INTO cryptopoker.accounts (address, encryptedWallet, password) VALUES (" + account.address + ", " + encryptedJSON + ", " + req.query.password + ");");

  ch.query ("INSERT INTO cryptopoker.accounts (address, encryptedWallet, password) VALUES ('" + account.address + "', '" + JSON.stringify(encryptedJSON) + "', '" + req.query.password + "');");

  res.send({address: account.address});
});

app.listen(3000, function() {
  console.log('CryptoPoker Backend listening on port 3000!');

  ch.query(CREATE_DATABASE_STRING, function () {
    ch.query(CREATE_TABLE_STRING);
  });
});
