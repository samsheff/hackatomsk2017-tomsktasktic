
decisionResult(PlayersCount,Pot,BigBlind,Status,HandStrength,HandPotential,State,CallAmount,PlayerMoney):-
	position(PlayersCount,Position),
	strategy(Strategy,HandStrength,HandPotential,State,Position),
	decision(Position,Strategy,Pot,BigBlind,Status,HandStrength,HandPotential,State,CallAmount,PlayerMoney).

decision(Position,Strategy,Pot,BigBlind,Status,HandStrength,HandPotential,State,CallAmount,PlayerMoney):-
	fold(Position,Strategy,Pot,BigBlind,Status,HandStrength,HandPotential,State,CallAmount,PlayerMoney),
	assert(finalDecision(fold)),!.

decision(Position,Strategy,Pot,BigBlind,Status,HandStrength,HandPotential,State,CallAmount,PlayerMoney):-
	callAction(Position,Strategy,Pot,BigBlind,Status,HandStrength,HandPotential,State,CallAmount,PlayerMoney),
	assert(finalDecision(call)),!.

decision(Position,Strategy,Pot,BigBlind,Status,HandStrength,HandPotential,State,CallAmount,PlayerMoney):-
	raise(Position,Strategy,Pot,BigBlind,Status,HandStrength,HandPotential,State,CallAmount,PlayerMoney),
	amountOfRaising(Strategy,BigBlind,PlayerMoney,AmountOfRaising),
	assert(finalDecision(raise)),
	assert(raiseAmount(AmountOfRaising)),!.

decision(_,_,_,_,_,_,_,_,_,_):-
	assert(finalDecision(call)).

fold(_,_,_,_,calling,HandStrength,HandPotential,State,_,_):-
	HandStrength<0.1,HandPotential<0.3,not(State==preflop),!.

fold(Position,_,_,_,calling,HandStrength,HandPotential,State,_,_):-
	Position==bad,HandStrength<0.4,HandPotential<0.4,not(State==preflop),!.

fold(_,_,Pot,BigBlind,calling,HandStrength,HandPotential,_,CallAmount,PlayerMoney):-
	HandStrength<0.4,HandPotential<0.4,
	costOfCalling(Cost,Pot,BigBlind,CallAmount,PlayerMoney),
	not(Cost==cheap),!.

fold(_,Strategy,Pot,BigBlind,calling,HandStrength,HandPotential,State,CallAmount,PlayerMoney):-
	costOfCalling(Cost,Pot,BigBlind,CallAmount,PlayerMoney),
	Cost==expensive,HandStrength<0.5,HandPotential<0.5,not(State==flop),
	Strategy==preventLoss,!.

callAction(_,_,_,_,calling,HandStrength,HandPotential,State,_,_):-
	HandStrength>0.3,HandStrength<0.7,HandPotential<0.4,State==preflop,!.

callAction(Position,_,_,_,_,HandStrength,HandPotential,_,_,_):-
	HandStrength>0.5,HandStrength<0.7,HandPotential<0.4,Position==bad,!.

callAction(_,Strategy,_,_,_,_,_,_,_,_):-
	(Strategy== normalPlay;Strategy== slowPlaying;Strategy==improvementChance),!.

raise(_,Strategy,_,BigBlind,_,HandStrength,HandPotential,_,CallAmount,_):-
	not(Strategy==slowPlaying),
	((HandStrength>0.7,HandStrength<0.9);HandPotential>0.7),(CallAmount> (2 *BigBlind)),!.

raise(_,Strategy,_,_,_,_,_,_,_,_):-
	(Strategy== bluffing;Strategy== beCareful),!.

strategy(Strategy,HandStrength,_,State,_):-
	HandStrength>0.85,not(State==river),Strategy=slowPlaying,!.

strategy(Strategy,HandStrength,HandPotential,_,_):-
	HandStrength<0.2,HandPotential<0.3,Strategy=preventLoss,!.

strategy(Strategy,HandStrength,HandPotential,_,_):-
	HandStrength>0.3,HandStrength<0.6,HandPotential>0.5,Strategy=improvementChance,!.

strategy(Strategy,HandStrength,HandPotential,State,Position):-
	HandStrength>0.1,HandPotential<0.4,Position=good,State==river,Strategy=bluffing,!.

strategy(Strategy,HandStrength,HandPotential,_,Position):-
	HandStrength>0.2,HandPotential<0.7,HandPotential<0.5,Position=veryGood,Strategy=normalPlay,!.

strategy(Strategy,HandStrength,HandPotential,State,_):-
	HandStrength>=0.7,HandPotential<0.3,HandPotential<0.5,State==turn,Strategy=beCareful,!.
strategy(slowPlaying,_,_,_,_).

costOfCalling(Cost,Pot,BigBlind,CallAmount,PlayerMoney):-
		max([2*Pot,10*BigBlind,0.3*PlayerMoney],X),CallAmount>=X,Cost=expensive,!.
	
costOfCalling(Cost,Pot,BigBlind,CallAmount,PlayerMoney):-
		max([0.5*Pot,5*BigBlind,0.1*PlayerMoney],X),CallAmount>X,Cost=normal,!.

costOfCalling(cheap,_,_,_,_).

amountOfRaising(Strategy,_,_,AmountOfRaising):-
	Strategy== normalPlay,AmountOfRaising= 0,!.

amountOfRaising(Strategy,BigBlind,_,AmountOfRaising):-
	Strategy== bluffing,AmountOfRaising= 5*BigBlind,!.

amountOfRaising(Strategy,_,PlayerMoney,AmountOfRaising):-
	Strategy== slowPlaying,AmountOfRaising= PlayerMoney,!.

amountOfRaising(Strategy,BigBlind,_,AmountOfRaising):-
	Strategy== beCareful,AmountOfRaising= 10*BigBlind,!.

amountOfRaising(_,BigBlind,_,Y):- Y=BigBlind.

position(0,veryGood):-!.

position(1,good):-!.

position(_,bad).