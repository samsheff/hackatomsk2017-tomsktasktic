evaluate(Hand):-
								myCards(Cards),calculateHand(Cards,Hand,_),!.

myCards(Cards)			:-		
								findall(Y,player(Y),Cards).

calculateHand(Cards,Hand,CardsEvaluated)		:-
												getCardsValuesAndSuits(Cards,Values,Suits),
												flushIsPotential(Suits),
												straightIsPotential(Values),
												solve(Cards,Suits,Values,Hand,CardsEvaluated).

solve(Cards,Suits,Values,Hand,CardsEvaluated):-
				prepareSolve(Cards,Suits,Values,Hand,CardsEvaluated1),
				(	(not(Hand==flush),not(Hand==straightFlush),not(Hand==royalStraightFlush),
					evaluateCardsFromValuesOrdered(Cards,CardsEvaluated1,CardsEvaluated));
					(CardsEvaluated= CardsEvaluated1)),saveResult(CardsEvaluated).

prepareSolve(Cards,Suits,Values,Hand,CardsEvaluatedByValue):-
				getFrequency(Values),
				findall(X,freq(X,1),UniqueSet),
				findall(Y,freq(Y,2),TwoOfKindSet),
				findall(Z,freq(Z,3),ThreeOfKindSet),
				findall(K,freq(K,4),FourOfKindSet),
				retractall(freq(_,_)),
				result(Hand,Cards,Suits,Values,UniqueSet,TwoOfKindSet,ThreeOfKindSet,FourOfKindSet,CardsEvaluatedByValue).

result(Hand,Cards,Suits,Values,UniqueSet,TwoOfKindSet,
			ThreeOfKindSet,FourOfKindSet,CardsEvaluatedByValue):-
				replace(1,14,Values,Values1),	
				getType(Hand,Cards,Suits,Values1,UniqueSet,TwoOfKindSet,
				ThreeOfKindSet,FourOfKindSet,CardsEvaluatedByValue).

saveResult(Cards):-
				saveValues(Cards),
				saveSuits(Cards).

saveValues(Cards):-
		append(Cards,[],[
			card(value(X1),suit(_)),
			card(value(X2),suit(_)),
			card(value(X3),suit(_)),
			card(value(X4),suit(_)),
			card(value(X5),suit(_))|_]),
		assert(cardValue(X1,1)),
		assert(cardValue(X2,2)),
		assert(cardValue(X3,3)),
		assert(cardValue(X4,4)),
		assert(cardValue(X5,5)).

saveSuits(Cards):-
		append(Cards,[],[
			card(value(_),suit(X1)),
			card(value(_),suit(X2)),
			card(value(_),suit(X3)),
			card(value(_),suit(X4)),
			card(value(_),suit(X5))|_]),
		assert(cardSuit(X1,1)),
		assert(cardSuit(X2,2)),
		assert(cardSuit(X3,3)),
		assert(cardSuit(X4,4)),
		assert(cardSuit(X5,5)).

getType(Hand,Cards,Suits,_,_,_,_,_,CardsEvaluated):-
		royalStraightFlush(Cards,Suits,CardsEvaluated),Hand = royalStraightFlush,!.

getType(Hand,Cards,Suits,Values,_,_,_,_,CardsEvaluated):-
		straightFlush(Cards,Values,Suits,CardsEvaluated),Hand = straightFlush,!.

getType(Hand,_,_,Values,_,_,_,FourOfKindSet,CardsEvaluatedByValue):-
		length(FourOfKindSet,1),
		append([Value],[],FourOfKindSet),
		(	(max(Values,Value),delete(Values,Value,Values1),max(Values1,Kicker));
			(max(Values,Kicker))),
		CardsEvaluatedByValue=[Value,Value,Value,Value,Kicker],
		Hand = fourOfKind,!.

getType(Hand,_,_,_,_,_,ThreeOfKindSet,_,CardsEvaluatedByValue):-
		length(ThreeOfKindSet,2),
		sort(ThreeOfKindSet,[X2,X1]),
		CardsEvaluatedByValue =[X1,X1,X1,X2,X2],
		Hand = fullHouse,!.

getType(Hand,_,_,_,_,TwoOfKindSet,ThreeOfKindSet,_,CardsEvaluatedByValue):-
		length(ThreeOfKindSet,1),length(TwoOfKindSet,X),X>0,
		max(TwoOfKindSet,X1),
		append([Value],[],ThreeOfKindSet),
		CardsEvaluatedByValue =[Value,Value,Value,X1,X1],
		Hand = fullHouse,!.

getType(Hand,Cards,Suits,_,_,_,_,_,EvaluatedCards):-
		solveForFLush(Cards,Suits,_,EvaluatedCards),
		Hand = flush,!.

getType(Hand,_,_,Values,_,_,_,_,CardsEvaluatedByValue):-
		CardsEvaluatedByValue = [10,11,12,13,1],membership(CardsEvaluatedByValue,Values),
		Hand = royalStraight,!.

getType(Hand,_,_,Values,_,_,_,_,CardsEvaluatedByValue):-
		solveForStraight(Values,CardsEvaluatedByValue),
		Hand = straight,!.

getType(Hand,_,_,_,UniqueSet,_,ThreeOfKindSet,_,CardsEvaluatedByValue):-
		length(ThreeOfKindSet,1),
		append([Value],[],ThreeOfKindSet),
		sort(UniqueSet,UniqueSetSorted),
		reverse(UniqueSetSorted,UniqueSetSortedReversed),
		append(UniqueSetSortedReversed,[],[FirstKicker,Second|_]),
		CardsEvaluatedByValue=[Value,Value,Value,FirstKicker,Second],
		Hand = threeOfKind,!.

getType(Hand,_,_,_,UniqueSet,TwoOfKindSet,_,_,CardsEvaluatedByValue):-
		length(TwoOfKindSet,X),X==3,
		sort(TwoOfKindSet,[X3,X2,X1]),
		max(UniqueSet,Kicker),
		(	Kicker1 = Kicker,Kicker>X3;
			Kicker1 = X3),
		CardsEvaluatedByValue=[X1,X1,X2,X2,Kicker1],
		Hand = twoPair,!.

getType(Hand,_,_,_,UniqueSet,TwoOfKindSet,_,_,CardsEvaluatedByValue):-
		length(TwoOfKindSet,X),X==2,
		sort(TwoOfKindSet,[X2,X1]),
		max(UniqueSet,Kicker),
		CardsEvaluatedByValue=[X1,X1,X2,X2,Kicker],
		Hand = twoPair,!.

getType(Hand,_,_,_,UniqueSet,TwoOfKindSet,_,_,CardsEvaluatedByValue):-
		length(TwoOfKindSet,1),
		append([Value],[],TwoOfKindSet),
		sort(UniqueSet,UniqueSetSorted),
		reverse(UniqueSetSorted,UniqueSetSortedReversed),
		append(UniqueSetSortedReversed,[],[FirstKicker,Second,Third|_]),
		CardsEvaluatedByValue=[Value,Value,FirstKicker,Second,Third],
		Hand = onePair,!.

getType(Hand,_,_,Values,_,_,_,_,CardsEvaluatedByValue):-
		sort(Values,Values1),
		reverse(Values1,Values2),
		append(Values2,[],[X1,X2,X3,X4,X5|_]),
		CardsEvaluatedByValue=[X1,X2,X3,X4,X5],
		Hand = highCard,!.
		
getFrequency(L):-
		replace(1,14,L,L1),
		sort(L1,L2),
		setFrequency(L2,L1).

setFrequency([],_):-!.
setFrequency([X|Y],L):-
				count(L,X,N),assert(freq(X,N)),
				setFrequency(Y,L).			

max([],0).
max([X],X).
max([X|Xs],X):- max(Xs,Y), X >=Y.
max([X|Xs],N):- max(Xs,N), N > X.

count([],_,0).
count([X|T],X,Y):- count(T,X,Z), Y is 1+Z.
count([X1|T],X,Z):- X1\=X,count(T,X,Z).

replaceCards(_, _, [], []).
replaceCards(Value, NewValue, [card(value(Value),suit(Y))|T], [card(value(NewValue),suit(Y))|T2]) :-
				replaceCards(Value, NewValue,T,T2),!.
replaceCards(Value, NewValue, [card(value(Value1),suit(Y))|T], [card(value(Value1),suit(Y))|T2]) :-
				replaceCards(Value, NewValue,T,T2),!.

replace(X,Y,L1,L2):-
					member(X,L1),
					replace1(X,Y,L1,L2),!.
replace(X,_,L1,L1):-
					not(member(X,L1)),!.

replace1(_, _, [], []).
replace1(O, R, [O|T], [R|T2]) :- replace1(O, R, T, T2).
replace1(O, R, [H|T], [H|T2]) :- H \= O, replace1(O, R, T, T2).

membership([],_).
membership([X|Y],L) :- member(X,L),membership(Y,L).

membershipCount([],_,0).
membershipCount([X|Y],L,Count):- member(X,L),membershipCount(Y,L,Count1),Count is Count1+1,!.
membershipCount([_|Y],L,Count):- membershipCount(Y,L,Count).

evaluateCardsFromValuesOrdered(Cards,Values,CardsEvaluated):-
					replace(14,1,Values,Values1),
					evaluateCardsFromValues(Values1,Cards,CardsEvaluated).

evaluateCardsFromValues([],_,[]):-!.
evaluateCardsFromValues([Value|Values],Cards,CardsEvaluated):-
					getCard(Value,Card,Cards,Cards1),
					evaluateCardsFromValues(Values,Cards1,CardsEvaluated1),
					CardsEvaluated=[Card|CardsEvaluated1],!.

getCard(Value,card(value(Value),suit(Y)),[card(value(Value),suit(Y))|Cards],Cards):-!.
getCard(Value,Card,[card(value(Value1),suit(Y))|Tail],Cards):-
					getCard(Value,Card,Tail,Cards1),
					Cards=[card(value(Value1),suit(Y))|Cards1],!.


deleteOneValue([],_,[]).
deleteOneValue(L,Value,L):-
			not(member(Value,L)),!.
deleteOneValue([Value|Values],Value,Values):-!.
deleteOneValue([Value1|Values],Value,Values1):-
								deleteOneValue(Values,Value,Values2),
								Values1=[Value1|Values2],!.

royalStraightFlush(Cards,Suits,CardsEvaluated) 	:-
						solveForFLush(Cards,Suits,HandSuit,_),
						CardsEvaluated=[
							card(value(10),suit(HandSuit)),
							card(value(11),suit(HandSuit)),
							card(value(12),suit(HandSuit)),
							card(value(13),suit(HandSuit)),
							card(value(1),suit(HandSuit))
						],
						membership(CardsEvaluated,Cards).

straightFlush(Cards,Values,Suits,CardsEvaluated)	:-
						solveForFLush(Cards,Suits,SuitValue,_),
						solveForStraight(Values,CardsEvaluatedByValue),
						replace(14,1,CardsEvaluatedByValue,CardsEvaluatedByValue1),
						append(CardsEvaluatedByValue1,[],[X1,X2,X3,X4,HighCard]),
						CardsEvaluated=[
									card(value(X1),suit(SuitValue)),
									card(value(X2),suit(SuitValue)),
									card(value(X3),suit(SuitValue)),
									card(value(X4),suit(SuitValue)),
									card(value(HighCard),suit(SuitValue))],
						membership(CardsEvaluated,Cards),!.

solveForFLush(Cards,Suits,SuitValue,EvaluatedCards):-
						flushAvailable(Suits),
						getSuitType(Suits,Suits,SuitValue),
						getFlushCards(Cards,SuitValue,FlushCards),
						getFlushCardsAsKickers(FlushCards,EvaluatedCards,SuitValue).

solveForStraight(Values,CardsEvaluatedByValue):-
						replace(14,1,Values,Values1),
						straightCase(CardsEvaluatedByValue),membership(CardsEvaluatedByValue,Values1).

flushAvailable(Suits):- sort(Suits,Suits1),length(Suits,X),length(Suits1,Y),X>Y+4.

getSuitType(Suits,[SuitValue|_],SuitValue):-	count(Suits,SuitValue,N),N>4,!.
getSuitType(Suits,[_|T],SuitValue):-	getSuitType(Suits,T,SuitValue).
getFlushCardsAsKickers(FlushCards,EvaluatedCards,SuitValue):-
					sort(FlushCards,FlushCards1),
					(
						(Z1=card(value(1),suit(SuitValue)),member(Z1,FlushCards1),delete(FlushCards1,Z1,FlushCards2),
						FlushCards3=[Z1|FlushCards2],
						append(FlushCards3,[],[X1,X2,X3,X4,X5|_]),
						EvaluatedCards=[X1,X5,X4,X3,X2]
						);
						(append(FlushCards1,[],[X1,X2,X3,X4,X5|_]),
						EvaluatedCards=[X5,X4,X3,X2,X1])
					),!.
					
getFlushCards([],_,[]).
getFlushCards([card(value(X),suit(SuitValue))|Cards],SuitValue,FlushCards):-
					getFlushCards(Cards,SuitValue,FlushCards1),
					FlushCards=[card(value(X),suit(SuitValue))|FlushCards1],!.
getFlushCards([_|Cards],SuitValue,FlushCards):-
					getFlushCards(Cards,SuitValue,FlushCards),!.

flushIsPotential(Suits):- sort(Suits,Suits1),length(Suits,X),length(Suits1,Y),(X==Y+4),assert(flushPotential(1)),!.
flushIsPotential(_):-assert(flushPotential(0)).

straightIsPotential(Values):-
						straightPotentialCase(CardsEvaluatedByValue),membership(CardsEvaluatedByValue,Values),
						assert(straightPotential(2)),!.

straightIsPotential(Values):-
						straightCase(CardsEvaluatedByValue),membershipCount(CardsEvaluatedByValue,Values,Count),
						Count==4,assert(straightPotential(1)),!.
						
straightIsPotential(Values):-
						membershipCount([10,11,12,13,1],Values,Count),Count==4,assert(straightPotential(1)),!.

straightIsPotential(_):-assert(straightPotential(0)).



straightCase([9,10,11,12,13]).	
straightCase([8,9,10,11,12]).
straightCase([7,8,9,10,11]).	
straightCase([6,7,8,9,10]).	
straightCase([5,6,7,8,9]).	
straightCase([4,5,6,7,8]).
straightCase([3,4,5,6,7]).	
straightCase([2,3,4,5,6]).
straightCase([1,2,3,4,5]).

straightPotentialCase([10,11,12,13]).
straightPotentialCase([9,10,11,12]).	
straightPotentialCase([8,9,10,11]).
straightPotentialCase([7,8,9,10]).	
straightPotentialCase([6,7,8,9]).	
straightPotentialCase([5,6,7,8]).	
straightPotentialCase([4,5,6,7]).
straightPotentialCase([3,4,5,6]).	
straightPotentialCase([2,3,4,5]).
straightPotentialCase([1,2,3,4]).

getCardsValuesAndSuits([],[],[]).
getCardsValuesAndSuits([card(value(X),suit(Y))|Cards],Values,Suits):-
										getCardsValuesAndSuits(Cards,Values1,Suits1),
										Values=[X|Values1],Suits=[Y|Suits1].