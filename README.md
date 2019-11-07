# Shooting with GA
shooting simulation with genetic algorithm

## Youtube
[Youtube](https://www.youtube.com/watch?v=e66OIfVDng4)

## Images
![](/Images/img1.png)

![](/Images/img2.png)

## Description
This simulation tries to solve the better path to reach the target with gentic algorithm.

## こだわりポイント (The point I put an effort)
- プロセスの可視化 (ja) <br>
遺伝的アルゴリズムは最適解を求める際のヒューリスティックな手法として用いられることが多い。本プロジェクトでも弾丸のターゲットへの最適経路を求める解法として遺伝的アルゴリズムを用いた。しかし自分は遺伝的アルゴリズムを用いることで結果として近似的最適解が得られることにはそこまで興奮しない。それよりも、アルゴリズムが近似的最適を導いていくプロセスの背後で働いているダイナミックスさの方に興奮する。そこをなんとか表現できないかと思い、あえて各世代のシミュレーション施行後に各個体の適応度、自然淘汰による交叉のプロセスをターミナルのようなUIで表現した。
- visualization of the GA process (en) <br>
Genetic algorithm is normally used for solving optimization problems approximately. In this project, I used genetic algorithm to search the better path for bullets to reach the target. I'm not interested in getting the optimized results by genetic algorithm, however. Rather than that, I'm interested in the dynamic movement happening behind the process which genetic algorithm tries to get the optimized results. Thus, I display the UI which shows the fitness of each indivisuals and the result of natural selection and crossover after the simulation of every generation.

## GA
### Crossover
- One point crossover
Chromsome is an array of vectors. Since the sequence of those vectors is important for this case, I use one point crossover which normally does not destroy the sequence that much. 
### Selection
- Roulette selection
This selection is simply following the thoght of "The closer the distance to the target is, the higher the fitness is".
This is a simple algorithm, but powerful to inherit the better genom.
