# Generates a file that file represents a random Python Dictionary
# Format [key,value]

import json
import random
import string

# Size is the number of dictionaries in the file
# minKeys is the minimum amount of keys the dictionary can have. Default 1
# maxKeys the is maximum amount of keys the dictionary can have
def generate(filename, size: int, minKeys: int, maxKeys: int):
    clearFile(filename)
    file = open(filename, 'a')
    delimiter = "|\n"
    # x is the number of dictionaries to make
    for x in range(size):
        key = "key"
        dictionary = {}
        numValues = random.randint(minKeys, maxKeys) #Gets number of values for dictionary
        # i is the amount of key-value pairs in the dictionary
        for i in range(numValues):
            valueType = random.randint(0, 3) # Determines what the value will be
            if valueType == 0: #Value will be boolean
                dictionary[key + str(i)] = randomBool()
            elif valueType == 1: # Value will be int
                dictionary[key + str(i)] = random.randint(0, 99)
            elif valueType == 2: # Value will be float rounded to two digits
                dictionary[key + str(i)] = round(random.uniform(0, 99), 2)
            else: # Value will be string
                dictionary[key + str(i)] = randomString()
        file.write(json.dumps(dictionary) + delimiter)
    file.close()

        



# Generates a random boolean
def randomBool() -> bool:
    if(random.randint(0,1) == 0):
        return False
    return True

# Generates a random string of lowercast letters
def randomString() -> str:
    letters = string.ascii_lowercase
    randLength = random.randint(5, 20) #String can be of length 5 to 20
    return ''.join(random.choice(letters) for x in range(randLength)) # Returns a random string

# Clears a file contents
def clearFile(filename):
    file = open(filename, 'w')
    file.write('')
    file.close()
    

generate("Random500.txt", 500, 10, 30)