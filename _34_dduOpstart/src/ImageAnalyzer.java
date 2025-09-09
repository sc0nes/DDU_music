import java.awt.image.BufferedImage;
import java.awt.Color;

public class ImageAnalyzer {
    public static Color getAverageColor(BufferedImage image){
        int height = image.getHeight(), width = image.getWidth();
        int n = width*height;
        long r = 0, g = 0, b = 0;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Color rgb = new Color(image.getRGB(x, y));
                r += rgb.getRed();
                g += rgb.getGreen();
                b += rgb.getBlue();
            }
        }
        Color c = new Color((int)(r/n),(int)(g/n),(int)(b/n));
        return c;
    }
    public static boolean isRed(BufferedImage image) {
        Color c = getAverageColor(image);
        int r = c.getRed(), g = c.getGreen(), b = c.getBlue();
        if(r>g&&r>b){
            return true;
        }
        return false;
    }
    public static boolean isGreen(BufferedImage image) {
        Color c = getAverageColor(image);
        int r = c.getRed(), g = c.getGreen(), b = c.getBlue();
        if(g>r&&g>b){
            return true;
        }
        return false;
    }
    public static boolean isBlue(BufferedImage image) {
        Color c = getAverageColor(image);
        int r = c.getRed(), g = c.getGreen(), b = c.getBlue();
        if(b>g&&b>r){
            return true;
        }
        return false;
    }
}
