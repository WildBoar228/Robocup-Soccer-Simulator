from PIL import Image


def transpar(num, name):
    im = Image.open(name)
    pixels = im.load()
    x, y = im.size
    
    im2 = Image.new("RGBA", (x, y), (0, 0, 0, 0))
    pixels2 = im2.load()

    for i in range(x):
        for j in range(y):
            if sum(pixels[i, j]) / len(pixels[i, j]) < num:
                pixels2[i, j] = pixels[i, j]
    
    im2.save('_' + name)


transpar(int(input()), input())
