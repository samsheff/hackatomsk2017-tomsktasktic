var Web3 = require('web3');
web3 = new Web3(new Web3.providers.HttpProvider(process.env.GETH_ADDRESS || "http://geth:8110"));

var clickhouse = require('@apla/clickhouse');
ch = new clickhouse(process.env.CLICKHOUSE_ADDRESS || "clickhouse");

const express = require('express')
const app = express()

const CREATE_DATABASE_STRING = "CREATE DATABASE IF NOT EXISTS cryptopoker;"
const CREATE_ACCOUNT_TABLE_STRING = "CREATE TABLE IF NOT EXISTS cryptopoker.accounts (address String, encryptedWallet String, password String, createdAt DateTime DEFAULT now()) Engine = Log;";
const CREATE_TABLES_TABLE_STRING = "CREATE TABLE IF NOT EXISTS cryptopoker.tables (id UInt32, name String, minimum_buy_in UInt32 DEFAULT 10, maximum_buy_in UInt32 DEFAULT 1000, small_blind UInt32 DEFAULT 6, big_blind UInt32 DEFAULT 3, max_players UInt32 DEFAULT 8) Engine = Log;";
const ADD_TEST_TABLE = "INSERT INTO cryptopoker.tables (id, name) VALUES (1, 'SuperCrypto');"

app.get('/balance', function(req, res) {
  var balance = web3.eth.getBalance(req.query.address).then(function(balance) {
    res.send({balance: balance.toString(10)});
  });
});

app.get('/register', function(req, res) {
  var account = web3.eth.accounts.create();
  var encryptedJSON = account.encrypt(account.privateKey, req.query.password);

  ch.query ("INSERT INTO cryptopoker.accounts (address, encryptedWallet, password) VALUES ('" + account.address + "', '" + JSON.stringify(encryptedJSON) + "', '" + req.query.password + "');");

  res.send({address: account.address});
});

app.get('/tables', function(req, res){
  var stream = ch.query('SELECT * FROM cryptopoker.tables');
  var tables = [];
  
  stream.on ('data', function (table) {
    tables.push (table);
  });

  stream.on ('end', function () {
    res.send({tables: tables});
  });
});



app.listen(3000, function() {
  console.log('CryptoPoker Backend listening on port 3000!');

  ch.query(CREATE_DATABASE_STRING, function () {
    ch.query(CREATE_ACCOUNT_TABLE_STRING, function () {
      ch.query(CREATE_TABLES_TABLE_STRING, function () {
        ch.query(ADD_TEST_TABLE);
      });
    });
  });
});
