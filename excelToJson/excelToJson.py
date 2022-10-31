from email.policy import default
import pandas as pd
import json as json
import numpy as np

class NpEncoder(json.JSONEncoder):
    def default(self, obj):
        if isinstance(obj, np.integer):
            return int(obj)
        elif isinstance(obj, np.floating):
            return float(obj)
        elif isinstance(obj, np.ndarray):
            return obj.tolist()
        else:
            return super(NpEncoder, self).default(obj)



LEVEL = "Level"
EVIL_HP = "Evil-HP"
EVIL_ATK = "Evil-ATK"
WARRIOR_HP = "Warrior-HP"
WARRIOR_ATK = "Warrior-ATK"
ARCHER_HP = "Archer-HP"
ARCHER_ATK = "Archer-ATK"
OUT_COINS = "OutCoins"

TOTAL_LEVEL = 100
TOTAL_HERO_LEVEL = 9

data = pd.read_excel("Value.xlsx")
# print(data)

# l = data["Level"]
# print(l[2])

info = {}

warrior = []
arhcer = []
game = []

for i in range(TOTAL_HERO_LEVEL):
    w = {}
    w['level'] = i
    w['HP'] = data[WARRIOR_HP][i]
    w['ATK'] = data[WARRIOR_ATK][i]
    warrior.append(w)

    a = {}
    a['level'] = i
    a['HP'] = data[ARCHER_HP][i]
    a['ATK'] = data[ARCHER_ATK][i]
    arhcer.append(a)


for i in range(TOTAL_LEVEL):
    g = {}
    g['level'] = i+1
    g['HP'] = data[EVIL_HP][i]
    g['ATK'] = data[EVIL_ATK][i]
    g['coins'] = data[OUT_COINS][i]
    game.append(g)

info['game'] = game
info['warrior'] = warrior
info['archer'] = arhcer

jsonStr = json.dumps(info, cls=NpEncoder, indent=4);
# print(jsonStr)

with open("Value.json", "w") as file:
    file.write(jsonStr)


