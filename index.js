var Web3 = require('web3');
web3 = new Web3(new Web3.providers.HttpProvider(process.env.GETH_ADDRESS || "http://geth:8110"));

var clickhouse = require('@apla/clickhouse');
ch = new clickhouse(process.env.CLICKHOUSE_ADDRESS || "clickhouse");

const express = require('express')
const app = express()

const CREATE_DATABASE_STRING = "CREATE DATABASE IF NOT EXISTS cryptopoker;"
const CREATE_PLAYER_TABLES_TABLE_STRING = "CREATE TABLE IF NOT EXISTS cryptopoker.player_tables (address_player String, id_table UInt32, createdAt DateTime DEFAULT now()) Engine = Log;";
const CREATE_ACCOUNT_TABLE_STRING = "CREATE TABLE IF NOT EXISTS cryptopoker.accounts (address String, encryptedWallet String, password String, createdAt DateTime DEFAULT now()) Engine = Log;";
const CREATE_TABLES_TABLE_STRING = "CREATE TABLE IF NOT EXISTS cryptopoker.tables (id UInt32, name String, minimum_buy_in UInt32 DEFAULT 10, maximum_buy_in UInt32 DEFAULT 1000, small_blind UInt32 DEFAULT 6, big_blind UInt32 DEFAULT 3, max_players UInt32 DEFAULT 8) Engine = Log;";
const CREATE_GAME_TABLE = "CREATE TABLE IF NOT EXISTS cryptopoker.game_table (id int NOT NULL AUTO_INCREMENT,address String, cash UInt32, bet UInt32 DEFAULT 0, first_card String, second_card String, card_3 String, card_4 String, card_5 String, card_6 String, card_7 String ) Engine = Log;";
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

app.get('/test', function(req, res){
  var buyin = req.query.buyin;
  res.send(buyin);
});

app.get('/start', function(req, res){
  var address = req.query.address;
  var balance = web3.eth.getBalance(req.query.address);
  var suits = new Array("H", "C", "S", "D");
  var cards = new Array();
  var cnt = 0;
  for(i=0; i<4; i++)
      for(j=1; j<=13; j++)
          cards[cnt++] = suits[i] + j;
  function randIntExcep(min, max, exp) {
      var n,
          exp = Array.isArray(exp) ? exp : [(isNaN(exp) ? min-1 : exp)];
      while(true){
          n = Math.floor(Math.random() * (max - min + 1)) + min;
          if(exp.indexOf(n) < 0) return n;
      }
  }
  var player_cards = [];
  var card_table = [];
  player_cards[0] = cards[randIntExcep(0,51)];
  player_cards[1] = cards[randIntExcep(0,51,player_cards[0])];
  ch.query('INSERT INTO cryptopoker.game_table (address,cash,first_card,second_card) VALUES ('+address+','+balance+','+player_cards[0]+','+player_cards[1]+')');
  res.send({card1: player_cards[0], card2: player_cards[1], address: address, balance: balance.toString(10)});
});

app.get('/raise', function(req, res){
  var bet = req.query.money;
  var address = req.query.address;  
  ch.query('UPDATE cryptopoker.game_table SET bet = '+bet+' WHERE address = '+address+'');
});

app.get('/check', function(req, res){
  var address = req.query.address;
  var exp = [];
  var card_table = [];
  var suits = new Array("H", "C", "S", "D");
  var cards = new Array();
  var cnt = 0;
  for(i=0; i<4; i++)
          for(j=1; j<=13; j++)
              cards[cnt++] = suits[i] + j;
      function randIntExcep(min, max, exp) {
          var n,
              exp = Array.isArray(exp) ? exp : [(isNaN(exp) ? min-1 : exp)];
          while(true){
              n = Math.floor(Math.random() * (max - min + 1)) + min;
              if(exp.indexOf(n) < 0) return n;
          }
      }
      exp[0] = ch.query('SELECT first_card FROM cryptopoker.game_table WHERE id = 1');
      exp[1] = ch.query('SELECT second_card FROM cryptopoker.game_table WHERE id = 1');
      exp[2] = ch.query('SELECT first_card FROM cryptopoker.game_table WHERE id = 2');
      for (var i = 3; i <= 7; i++) {
        exp[i] = ch.query('SELECT card_'+i+' FROM cryptopoker.game_table WHERE id = 1');
      }
  if(req.query.number == 3){
      for(var i = 3; i <= 5; i++){
        ch.query('UPDATE cryptopoker.game_table SET card_'+i+' = '+card_table[i]+' WHERE address = '+address+'');
      }
    res.send({card1: card_table[1]});
  }
  if(req.query.number = 5){
    for(var i = 5; i <= 7; i++){
      ch.query('UPDATE cryptopoker.game_table SET card_'+i+' = '+card_table[i]+' WHERE address = '+address+'');
    }
  }
  res.send({Answer: 'Next player'});
});

app.get('/getCards', function(req, res){
  var address = req.query.address;
  var number = req.query.number;
  var cards = [];
  if(number = 3){
    for (var i = 3; i <=5; i++) {
      cards[i] = ch.query('SELECT card_'+i+' FROM cryptopoker.game_table WHERE id = 1');
    }
    res.send({card_1: cards[3],card_2: cards[4],card_3: cards[5]});
  }
  if(number = 5){
    for (var i = 6; i <=7; i++) {
      cards[i] = ch.query('SELECT card_'+i+' FROM cryptopoker.game_table WHERE id = 1');
    }
    res.send({card_1: cards[6],card_2: cards[7]});
  }
});

app.get('/sit', function(req, res){
  var buyin = req.query.buyin || 0;
  var tableId = req.query.table;
  var address = req.query.address;

  if (buyin > 0 && tableId && address) {
    var tablesStream = ch.query('SELECT * FROM cryptopoker.tables WHERE id = ' + tableId + ' LIMIT 1');
    var table;

    tablesStream.on ('data', function (tableRow) {
      table = tableRow;
    });

    tablesStream.on ('end', function () {
      var minBuyIn = table[2];
      var maxBuyIn = table[3];
      if (buyin >= minBuyIn && buyin <= maxBuyIn) {
        ch.query("INSERT INTO cryptopoker.player_tables (address_player, id_table) VALUES ( '" + address + "', " + tableId + " LIMIT 1", function () {
          res.send({table: table});
        });
      } else {
        res.send({error: "Buy In Too Low"})
      }
    });
  } else {
    res.send({error: "Missing a required param!"})
  }
});

app.listen(3000, function() {
  console.log('CryptoPoker Backend listening on port 3000!');

  ch.query(CREATE_DATABASE_STRING, function () {
    ch.query(CREATE_ACCOUNT_TABLE_STRING, function () {
      ch.query(CREATE_TABLES_TABLE_STRING, function () {
        ch.query(ADD_TEST_TABLE, function () {
          ch.query(CREATE_PLAYER_TABLES_TABLE_STRING);
        });
      });
    });
  });
});
