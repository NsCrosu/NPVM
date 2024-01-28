import os  
from PIL import Image  

# 定义目标目录和输出目录  
input_dir = './'  # 输入图片所在的目录  
output_dir = './Input'  # 调整大小后的图片保存目录  

# 检查输出目录是否存在，如果不存在则创建  
if not os.path.exists(output_dir):  
    os.makedirs(output_dir)  

# 遍历输入目录中的所有文件  
for filename in os.listdir(input_dir):  
    # 检查文件是否为图片  
    if filename.endswith('.jpg'):  
        # 打开图片并调整大小  
        img = Image.open(os.path.join(input_dir, filename))
        img = img.convert('RGB')
        resized_img = img.resize((1000, 1000), Image.LANCZOS)
        # 保存调整大小后的图片到输出目录  
        resized_img.save(os.path.join(output_dir, filename))