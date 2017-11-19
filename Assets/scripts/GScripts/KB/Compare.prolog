result(Rank1,Rank2,CardsEvaluated1,CardsEvaluated2,Result):-
	replace(1,14,CardsEvaluated1,CardsEvaluated11),
	replace(1,14,CardsEvaluated2,CardsEvaluated22),
	compareHand(Rank1,Rank2,CardsEvaluated11,CardsEvaluated22,Result).

compareHand(Rank1,Rank2,_,_,Result):-
	Rank1>Rank2,Result=2,!.

compareHand(Rank1,Rank2,_,_,Result):-
	Rank1<Rank2,Result=0,!.

compareHand(Rank1,_,CardsEvaluated1,CardsEvaluated2,Result):-
	(Rank1==1;Rank1==7),
	compareFiveKickers(CardsEvaluated1,CardsEvaluated2,Result),!.

compareHand(2,_,[X|_],[Y|_],Result):-
	compareCard(X,Y,Result),not(Result==1),!.

compareHand(2,_,[_,_,X1|_],[_,_,Y1|_],Result):-
	compareCard(X1,Y1,Result),not(Result==1),!.

compareHand(2,_,[_,_,_,X2|_],[_,_,_,Y2|_],Result):-
	compareCard(X2,Y2,Result),not(Result==1),!.

compareHand(2,_,[_,_,_,_,X3|_],[_,_,_,_,Y3|_],Result):-
	compareCard(X3,Y3,Result),!.

compareHand(3,_,[X|_],[Y|_],Result):-
	compareCard(X,Y,Result),not(Result==1),!.

compareHand(3,_,[_,_,X|_],[_,_,Y|_],Result):-
	compareCard(X,Y,Result),not(Result==1),!.

compareHand(3,_,[_,_,_,_,X|_],[_,_,_,_,Y|_],Result):-
	compareCard(X,Y,Result),!.

compareHand(4,_,[X|_],[Y|_],Result):-
	compareCard(X,Y,Result),not(Result==1),!.

compareHand(4,_,[_,_,_,X|_],[_,_,_,Y|_],Result):-
	compareCard(X,Y,Result),not(Result==1),!.

compareHand(4,_,[_,_,_,_,X|_],[_,_,_,_,Y|_],Result):-
	compareCard(X,Y,Result),!.

compareHand(Rank1,_,[_,_,_,_,X|_],[_,_,_,_,Y|_],Result):-
	(Rank1==5;Rank1==10),compareCard(X,Y,Result),!.

compareHand(6,_,_,_,Result):-
	Result=1,!.

compareHand(8,_,[X|_],[Y|_],Result):-
	compareCard(X,Y,Result),not(Result==1),!.

compareHand(8,_,[_,_,_,X|_],[_,_,_,Y|_],Result):-
	compareCard(X,Y,Result),!.

compareHand(9,_,[X|_],[Y|_],Result):-
	compareCard(X,Y,Result),not(Result==1),!.

compareHand(9,_,[_,_,_,_,X|_],[_,_,_,_,Y|_],Result):-
	compareCard(X,Y,Result),!.


compareFiveKickers([X1|_],[Y1|_],Result):-
	compareCard(X1,Y1,Result),not(Result==1),!.
compareFiveKickers([_,X2|_],[_,Y2|_],Result):-
	compareCard(X2,Y2,Result),not(Result==1),!.
compareFiveKickers([_,_,X3|_],[_,_,Y3|_],Result):-
	compareCard(X3,Y3,Result),not(Result==1),!.
compareFiveKickers([_,_,_,X4|_],[_,_,_,Y4|_],Result):-
	compareCard(X4,Y4,Result),not(Result==1),!.
compareFiveKickers([_,_,_,_,X5|_],[_,_,_,_,Y5|_],Result):-
	compareCard(X5,Y5,Result).


compareCard(X,Y,Result):-
	(X>Y,Result=2);(X<Y,Result=0);(Result=1).

replace(X,Y,L1,L2):-
					member(X,L1),
					replace1(X,Y,L1,L2),!.
replace(X,_,L1,L1):-
					not(member(X,L1)),!.

replace1(_, _, [], []).
replace1(O, R, [O|T], [R|T2]) :- replace1(O, R, T, T2).
replace1(O, R, [H|T], [H|T2]) :- H \= O, replace1(O, R, T, T2).