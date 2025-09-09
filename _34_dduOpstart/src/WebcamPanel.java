import java.awt.image.BufferedImage;
import com.github.sarxos.webcam.Webcam;
import com.github.sarxos.webcam.WebcamResolution;
public class WebcamPanel extends javax.swing.JPanel implements Runnable {
    public volatile boolean running = true;

    private BufferedImage image;
    private Webcam webcam;
    WebcamPanel(){
        this.setPreferredSize(WebcamResolution.VGA.getSize());
        webcam = Webcam.getDefault();
        webcam.setViewSize(WebcamResolution.VGA.getSize());
        webcam.open();
    }
    public void run(){
        while(running){
            try{
                image = webcam.getImage();
                repaint();
                Thread.sleep(100);
            } catch(Exception e){
                running = false;
                e.printStackTrace();
            }
        }
        webcam.close();
    }

    public void paintComponent(java.awt.Graphics g){
        super.paintComponent(g);
        g.drawImage(image, 0, 0, this.getWidth(), this.getHeight(), null);
    }
    public BufferedImage getImage(){
        return image;
    }
}
